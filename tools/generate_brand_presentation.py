#!/usr/bin/env python3
"""Ezgi Craft — stacked geometric lockup, 14-slide identity presentation."""

from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image, ImageDraw, ImageEnhance, ImageFilter, ImageFont

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "tools"))
from ezgi_lockup import export_pack, render_e_bars_only  # noqa: E402

ASSETS = ROOT / "brand_assets"
WWW = ROOT / "src" / "ezgi_mobilya.Web" / "wwwroot"
OUT_DIR = ASSETS
LOCKUP_DIR = ASSETS / "lockup"
DESKTOP = Path(r"C:\Users\Fırat\Desktop")

W, H = 1920, 1080

GOLD = (197, 162, 107)
GOLD_DEEP = (138, 112, 67)
BLACK = (17, 17, 17)
WHITE = (255, 255, 255)
CREAM = (250, 246, 240)
SAND = (243, 236, 227)
CHARCOAL = (44, 37, 32)
TERRACOTTA = (194, 84, 26)
SAGE = (74, 99, 81)
MUTED = (102, 87, 82)
LINE = (230, 221, 213)

FONT_DISPLAY = Path(r"C:\Windows\Fonts\gothicb.ttf")
FONT_DISPLAY_REG = Path(r"C:\Windows\Fonts\gothic.ttf")
FONT_UI = Path(r"C:\Windows\Fonts\segoeui.ttf")
FONT_UI_BOLD = Path(r"C:\Windows\Fonts\segoeuib.ttf")
FONT_UI_LIGHT = Path(r"C:\Windows\Fonts\segoeuil.ttf")


def font(path: Path, size: int) -> ImageFont.FreeTypeFont:
    return ImageFont.truetype(str(path), size=size)


def new_slide(bg: tuple[int, int, int]) -> Image.Image:
    return Image.new("RGBA", (W, H), bg + (255,))


def draw(img: Image.Image) -> ImageDraw.ImageDraw:
    return ImageDraw.Draw(img)


def text_w(d: ImageDraw.ImageDraw, text: str, fnt: ImageFont.FreeTypeFont) -> float:
    return d.textlength(text, font=fnt)


def wrap(d: ImageDraw.ImageDraw, text: str, fnt: ImageFont.FreeTypeFont, max_w: float) -> list[str]:
    words = text.split()
    lines: list[str] = []
    cur = ""
    for word in words:
        test = f"{cur} {word}".strip()
        if text_w(d, test, fnt) <= max_w:
            cur = test
        else:
            if cur:
                lines.append(cur)
            cur = word
    if cur:
        lines.append(cur)
    return lines


def draw_wrapped(
    d: ImageDraw.ImageDraw,
    text: str,
    fnt: ImageFont.FreeTypeFont,
    xy: tuple[float, float],
    max_w: float,
    fill: tuple[int, int, int],
    leading: float = 1.45,
) -> float:
    x, y = xy
    lines = wrap(d, text, fnt, max_w)
    bbox = fnt.getbbox("Ag")
    lh = (bbox[3] - bbox[1]) * leading
    for i, line in enumerate(lines):
        d.text((x, y + i * lh), line, font=fnt, fill=fill)
    return y + len(lines) * lh


def load_logo(name: str) -> Image.Image:
    return Image.open(LOCKUP_DIR / name).convert("RGBA")


def fit(im: Image.Image, box_w: int, box_h: int) -> Image.Image:
    scale = min(box_w / im.width, box_h / im.height)
    nw, nh = max(1, int(im.width * scale)), max(1, int(im.height * scale))
    return im.resize((nw, nh), Image.Resampling.LANCZOS)


def paste_box(canvas: Image.Image, im: Image.Image, box: tuple[int, int, int, int]) -> None:
    x0, y0, x1, y1 = box
    w, h = max(1, x1 - x0), max(1, y1 - y0)
    fitted = fit(im, w, h)
    px = x0 + (w - fitted.width) // 2
    py = y0 + (h - fitted.height) // 2
    canvas.alpha_composite(fitted, (px, py))


def rounded_rect(d, box, r, fill=None, outline=None, width: int = 1) -> None:
    d.rounded_rectangle(box, radius=r, fill=fill, outline=outline, width=width)


def footer(img: Image.Image, page: int, total: int = 14, dark: bool = False) -> None:
    d = draw(img)
    fnt = font(FONT_UI, 16)
    color = (200, 196, 190) if dark else MUTED
    d.text((80, H - 48), "©2026 Ezgi Craft", font=fnt, fill=color)
    num = f"{page:02d}  /  {total:02d}"
    d.text((W - 80 - text_w(d, num, fnt), H - 48), num, font=fnt, fill=color)
    d.line((80, H - 68, W - 80, H - 68), fill=BLACK if not dark else (80, 76, 72), width=1)


def kicker(d, text: str, x: int, y: int, dark: bool = False) -> None:
    fnt = font(FONT_UI_BOLD, 13)
    d.text((x, y), text.upper(), font=fnt, fill=(180, 180, 176) if dark else MUTED)


def heading(d, text: str, x: int, y: int, size: int = 48, fill=CHARCOAL) -> float:
    fnt = font(FONT_DISPLAY, size)
    d.text((x, y), text, font=fnt, fill=fill)
    bbox = fnt.getbbox(text)
    return y + (bbox[3] - bbox[1]) + 18


def card(img, box, bg=WHITE, r: int = 8) -> None:
    rounded_rect(draw(img), box, r, fill=bg)


def product(rel: str) -> Image.Image:
    return Image.open(WWW / "images" / "products" / rel).convert("RGBA")


def cover_crop(im: Image.Image, w: int, h: int) -> Image.Image:
    scale = max(w / im.width, h / im.height)
    nw, nh = int(im.width * scale), int(im.height * scale)
    im = im.resize((nw, nh), Image.Resampling.LANCZOS)
    left = (nw - w) // 2
    top = (nh - h) // 2
    return im.crop((left, top, left + w, top + h))


# Tight crops that keep wood and structure, drop TVs / windows / clutter.
# box = (left, top, right, bottom) as fractions of the source.
PHOTOS: dict[str, tuple[str, tuple[float, float, float, float]]] = {
    "collection": ("dogal-ahsap-masa.png", (0.00, 0.28, 0.62, 0.92)),
    "material": ("klasik-ayakli-masa.png", (0.16, 0.30, 0.62, 0.62)),
    "atelier": ("bahce-masasi.png", (0.00, 0.38, 0.52, 0.98)),
    "hero": ("dogal-ahsap-masa.png", (0.00, 0.24, 0.62, 0.92)),
    "grain": ("klasik-ayakli-masa.png", (0.20, 0.34, 0.58, 0.62)),
}


def crop_frac(im: Image.Image, box: tuple[float, float, float, float]) -> Image.Image:
    w, h = im.size
    l, t, r, b = box
    return im.crop((int(w * l), int(h * t), int(w * r), int(h * b)))


def grade_photo(im: Image.Image, *, darken: float = 0.0) -> Image.Image:
    """Warm, slightly desaturated grade that sits with keten / kum / siyah."""
    rgb = im.convert("RGB")
    rgb = ImageEnhance.Color(rgb).enhance(0.58)
    rgb = Image.blend(rgb, Image.new("RGB", rgb.size, (244, 232, 214)), 0.16)
    rgb = ImageEnhance.Contrast(rgb).enhance(1.18)
    rgb = ImageEnhance.Brightness(rgb).enhance(0.90)
    rgb = ImageEnhance.Sharpness(rgb).enhance(1.08)
    if darken > 0:
        rgb = Image.blend(rgb, Image.new("RGB", rgb.size, (22, 20, 18)), darken)
    return rgb.convert("RGBA")


def photo(key: str, w: int, h: int, *, darken: float = 0.0) -> Image.Image:
    rel, box = PHOTOS[key]
    return cover_crop(grade_photo(crop_frac(product(rel), box), darken=darken), w, h)


def paste_rounded(canvas: Image.Image, im: Image.Image, xy: tuple[int, int], radius: int = 0) -> None:
    x, y = xy
    if radius <= 0:
        canvas.alpha_composite(im.convert("RGBA"), (x, y))
        return
    w, h = im.size
    mask = Image.new("L", (w, h), 0)
    ImageDraw.Draw(mask).rounded_rectangle((0, 0, w, h), radius, fill=255)
    layer = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    layer.paste(im.convert("RGBA"), mask=mask)
    canvas.alpha_composite(layer, (x, y))


def _recolor(im: Image.Image, rgb: tuple[int, int, int]) -> Image.Image:
    out = Image.new("RGBA", im.size, (0, 0, 0, 0))
    src = im.convert("RGBA")
    sp, op = src.load(), out.load()
    for y in range(src.height):
        for x in range(src.width):
            _, _, _, a = sp[x, y]
            if a:
                op[x, y] = (*rgb, a)
    return out


# ── Slides ───────────────────────────────────────────────────────────────────

def slide_01() -> Image.Image:
    img = new_slide(WHITE)
    d = draw(img)
    d.rectangle((0, 0, 12, H), fill=BLACK)
    kicker(d, "Brand Identity  ·  2026", 80, 72)
    paste_box(img, load_logo("lockup-black.png"), (80, 180, 900, 860))

    d.line((1020, 240, 1020, 780), fill=LINE, width=1)
    d.text((1080, 300), "Logo Design", font=font(FONT_DISPLAY_REG, 32), fill=CHARCOAL)
    d.text((1080, 348), "Presentation", font=font(FONT_DISPLAY_REG, 32), fill=CHARCOAL)
    draw_wrapped(
        d,
        "Üç çubuk. Açık iskelet. Dikey kilit. Mobilya gibi kurulur: katman katman, ölçüyle, fazlalıksız.",
        font(FONT_UI, 20),
        (1080, 460),
        700,
        MUTED,
        1.55,
    )
    d.text((1080, 640), "FURNITURE  &  OBJECTS", font=font(FONT_DISPLAY_REG, 16), fill=CHARCOAL)
    footer(img, 1)
    return img


def slide_02() -> Image.Image:
    img = new_slide(CREAM)
    d = draw(img)
    kicker(d, "Marka Hakkında  ·  ©2026 Ezgi Craft", 80, 56)
    heading(d, "Ezgi Craft", 80, 92)

    body = font(FONT_UI, 22)
    col_w = 820
    p1 = (
        "Ezgi Craft, doğal ahşabı el işçiliğiyle biçimlendiren bir atölye markasıdır. "
        "1997’den bu yana seri üretimin sıradanlığından uzak durur; her parça benzersiz damar, "
        "kesim ve dokunuş taşır."
    )
    p2 = (
        "İsim iki katmandır: Ezgi bir imza, Craft bir yöntem. Furniture & Objects ise çerçevenin "
        "içini doldurur — masa, sehpa, nesne; hepsi aynı atölye disiplininden çıkar."
    )
    p3 = (
        "Logo bu kurulumu tekrar eder. Üç yatay çubuk bir E’dir ve aynı zamanda istiflenmiş "
        "ahşap katmandır. Hiçbir omurga gizlenmez; yapı görünür."
    )
    y = draw_wrapped(d, p1, body, (80, 200), col_w, CHARCOAL, 1.5)
    y = draw_wrapped(d, p2, body, (80, y + 22), col_w, MUTED, 1.5)
    draw_wrapped(d, p3, body, (80, y + 22), col_w, MUTED, 1.5)
    d.text((80, 860), "El üretir, zaman onaylar.", font=font(FONT_DISPLAY, 26), fill=CHARCOAL)

    pillars = [
        ("01", "Doğal Ahşap", "Kayın, ceviz, zeytin ve kütük. Sürdürülebilir orman, yaşayan damar."),
        ("02", "El İşçiliği", "Hassas kesim, el zımparası, doğal yağ. Her parça atölyede tamamlanır."),
        ("03", "Geometrik Sadelik", "Omurga yok, süs yok. Form, ölçü ve boşluk konuşur."),
    ]
    card_x = 980
    for i, (num, title, desc) in enumerate(pillars):
        cy = 200 + i * 230
        card(img, (card_x, cy, 1840, cy + 210), WHITE, 8)
        d2 = draw(img)
        d2.text((card_x + 36, cy + 28), num, font=font(FONT_DISPLAY, 32), fill=CHARCOAL)
        d2.text((card_x + 36, cy + 82), title, font=font(FONT_UI_BOLD, 22), fill=CHARCOAL)
        draw_wrapped(d2, desc, font(FONT_UI, 17), (card_x + 36, cy + 118), 780, MUTED, 1.4)

    footer(img, 2)
    return img


def slide_03() -> Image.Image:
    img = new_slide(CREAM)
    d = draw(img)
    kicker(d, "Ezgi Craft Logosu ve Hikâyesi  ·  ©2026 Ezgi Craft", 80, 56)
    heading(d, "Katman, iskelet, ölçü", 80, 92, 44)

    paras = [
        "Kilit dikeydir çünkü mobilya da öyle kurulur: üstte imza, altında yöntem, "
        "en altta tarih. Hiçbir satır diğerinden koparılamaz.",
        "E üç eşit çubuktur — tezgâha istiflenmiş üç kereste. Dikey omurga yoktur; "
        "taşıyıcı görünür. CRAFT’taki A, çaprazı olmayan bir iskelettir: açık çerçeve.",
        "İnce İ, Türkçe imzanın noktasıdır. Alt çizgi tezgâhın ölçüm hattı, "
        "tireler birleşim yeridir. FURNITURE & OBJECTS çerçevenin içeriğini adlandırır.",
    ]
    y = 190
    for p in paras:
        y = draw_wrapped(d, p, font(FONT_UI, 20), (80, y), 760, CHARCOAL if y < 250 else MUTED, 1.48) + 22

    card(img, (920, 170, 1840, 900), WHITE, 8)
    paste_box(img, load_logo("lockup-black.png"), (980, 210, 1780, 780))
    d2 = draw(img)
    d2.text((980, 820), "ANA LOGO", font=font(FONT_UI_BOLD, 14), fill=CHARCOAL)
    d2.text((980, 846), "Siyah  ·  şeffaf zemin  ·  dikey kilit", font=font(FONT_UI, 16), fill=MUTED)
    footer(img, 3)
    return img


def slide_04() -> Image.Image:
    img = new_slide(CREAM)
    d = draw(img)
    kicker(d, "Logo Versiyonları  ·  ©2026 Ezgi Craft", 80, 48)
    heading(d, "Tek kilit, doğru zemin", 80, 80, 40)
    draw_wrapped(
        d,
        "Tüm versiyonlar aynı oranlara ve iç düzene sahiptir. Yazı, tireler ve EST. tarihi hiçbir versiyondan çıkarılmaz. "
        "Değişen tek şey renk ve kapsayıcıdır. Logo paleti yalnızca siyah ve beyazdır.",
        font(FONT_UI, 18),
        (80, 145),
        1760,
        MUTED,
        1.4,
    )

    tiles = [
        ("lockup-black.png", CREAM, "Ana Logo", "Siyah · açık zemin"),
        ("lockup-white.png", BLACK, "Beyaz", "Koyu zemin ve fotoğraf üzeri"),
        ("lockup-on-white.png", WHITE, "Beyaz kapsayıcı", "Varsayılan kart / belge"),
        ("lockup-on-black.png", BLACK, "Siyah kapsayıcı", "Negatif blok"),
        ("lockup-compact-black.png", CREAM, "Kompakt", "Küçük boy; tagline düşer, EST. kalır"),
        ("e-black.png", CREAM, "EZGİ Mark", "İsim damgası · menü değil"),
        ("e-white.png", BLACK, "App icon", "EZGİ wordmark · koyu kare"),
        ("lockup-compact-white.png", BLACK, "Kompakt beyaz", "Nav, footer, ambalaj"),
    ]
    gap, cols = 22, 4
    tile_w = (1760 - gap * (cols - 1)) // cols
    ox, oy = 80, 230
    for i, (path, bg, title, sub) in enumerate(tiles):
        c, r = i % cols, i // cols
        x = ox + c * (tile_w + gap)
        y = oy + r * 358
        rounded_rect(d, (x, y, x + tile_w, y + 250), 8, fill=bg)
        paste_box(img, load_logo(path), (x + 24, y + 20, x + tile_w - 24, y + 230))
        d.text((x, y + 262), title, font=font(FONT_UI_BOLD, 16), fill=CHARCOAL)
        d.text((x, y + 286), sub, font=font(FONT_UI, 14), fill=MUTED)
    footer(img, 4)
    return img


def slide_05() -> Image.Image:
    img = new_slide(CREAM)
    d = draw(img)
    kicker(d, "Koruma Alanı ve Minimum Boyut  ·  ©2026 Ezgi Craft", 80, 56)
    heading(d, "Çubuk ölçer, boşluk korur", 80, 92, 40)
    draw_wrapped(
        d,
        "Logo her zaman aynı oranlarda kullanılır. Etrafında, E çubuğunun kalınlığı kadar boş koruma alanı bırakılır; "
        "bu alana tipografi, görsel veya çerçeve giremez.",
        font(FONT_UI, 20),
        (80, 170),
        1760,
        MUTED,
        1.45,
    )

    card(img, (80, 280, 1100, 920), WHITE, 8)
    logo = load_logo("lockup-black.png")
    fitted = fit(logo, 620, 420)
    lx, ly = 80 + (1020 - fitted.width) // 2, 340
    img.alpha_composite(fitted, (lx, ly))
    x_pad = int(fitted.height * 0.08)
    bx0, by0 = lx - x_pad, ly - x_pad
    bx1, by1 = lx + fitted.width + x_pad, ly + fitted.height + x_pad
    d2 = draw(img)
    d2.rectangle((bx0, by0, bx1, by1), outline=BLACK, width=1)
    xf = font(FONT_UI_BOLD, 14)
    d2.text(((bx0 + bx1) / 2 - 8, by0 - 26), "X", font=xf, fill=CHARCOAL)
    d2.text(((bx0 + bx1) / 2 - 8, by1 + 8), "X", font=xf, fill=CHARCOAL)
    d2.text((bx0 - 22, (by0 + by1) / 2 - 8), "X", font=xf, fill=CHARCOAL)
    d2.text((bx1 + 8, (by0 + by1) / 2 - 8), "X", font=xf, fill=CHARCOAL)
    d2.text((lx, 820), "X  =  E çubuğunun kalınlığı", font=font(FONT_UI, 16), fill=MUTED)

    card(img, (1140, 280, 1840, 575), WHITE, 8)
    d2.text((1180, 310), "Dijital", font=font(FONT_UI_BOLD, 18), fill=CHARCOAL)
    img.alpha_composite(fit(logo, 280, 160), (1180, 350))
    d2.text((1180, 520), "140 px yükseklik", font=font(FONT_UI, 16), fill=MUTED)

    card(img, (1140, 600, 1840, 920), WHITE, 8)
    d2.text((1180, 630), "Baskı", font=font(FONT_UI_BOLD, 18), fill=CHARCOAL)
    img.alpha_composite(fit(logo, 320, 180), (1180, 670))
    d2.text((1180, 860), "28 mm yükseklik", font=font(FONT_UI, 16), fill=MUTED)
    footer(img, 5)
    return img


def slide_06() -> Image.Image:
    img = new_slide(CREAM)
    d = draw(img)
    kicker(d, "Farklı Zeminlerde Kullanım  ·  ©2026 Ezgi Craft", 80, 48)
    heading(d, "Zemin değişir, kilit değişmez", 80, 80, 40)
    draw_wrapped(
        d,
        "Açık ve nötr zeminlerde siyah kilit kullanılır. Koyu zemin, fotoğraf ve doygun marka renkleri üzerinde "
        "yalnızca beyaz versiyon seçilir. Logo asla altın, terracotta veya adaçayı ile boyanmaz.",
        font(FONT_UI, 18),
        (80, 145),
        1760,
        MUTED,
        1.4,
    )

    wood = photo("grain", 400, 210, darken=0.32)
    items = [
        (CREAM, "lockup-black.png", "Keten krem · siyah"),
        (WHITE, "lockup-black.png", "Beyaz · siyah"),
        (SAND, "lockup-black.png", "Kum · siyah"),
        (BLACK, "lockup-white.png", "Siyah · beyaz"),
        ((230, 230, 226), "lockup-black.png", "Açık gri · siyah"),
        (TERRACOTTA, "lockup-white.png", "Terracotta · beyaz"),
        (SAGE, "lockup-white.png", "Adaçayı · beyaz"),
        ("photo", "lockup-white.png", "Görsel üzeri · beyaz"),
    ]
    gap, cols = 20, 4
    tw = (1760 - gap * 3) // 4
    th = 250
    ox, oy = 80, 240
    for i, (bg, path, label) in enumerate(items):
        c, r = i % cols, i // cols
        x = ox + c * (tw + gap)
        y = oy + r * (th + 90)
        if bg == "photo":
            ph = wood.resize((tw, th), Image.Resampling.LANCZOS)
            mask = Image.new("L", (tw, th), 0)
            ImageDraw.Draw(mask).rounded_rectangle((0, 0, tw, th), 6, fill=255)
            rounded = Image.new("RGBA", (tw, th), (0, 0, 0, 0))
            rounded.paste(ph, mask=mask)
            img.alpha_composite(rounded, (x, y))
            overlay = Image.new("RGBA", (tw, th), (0, 0, 0, 0))
            ov = Image.new("RGBA", (tw, th), (17, 17, 17, 80))
            overlay.paste(ov, mask=mask)
            img.alpha_composite(overlay, (x, y))
        else:
            rounded_rect(d, (x, y, x + tw, y + th), 6, fill=bg)
        paste_box(img, load_logo(path), (x + 28, y + 28, x + tw - 28, y + th - 28))
        d.text((x, y + th + 12), label, font=font(FONT_UI, 15), fill=CHARCOAL)
    footer(img, 6)
    return img


def slide_07() -> Image.Image:
    img = new_slide(CREAM)
    d = draw(img)
    kicker(d, "Yanlış Kullanımlar  ·  ©2026 Ezgi Craft", 80, 48)
    heading(d, "Logo asla yeniden kurgulanmaz", 80, 80, 40)
    draw_wrapped(
        d,
        "Oranları sabittir. E’ye omurga eklenmez, A’ya çapraz konmaz, üç çubuk menü ikonu gibi tek başına kullanılmaz.",
        font(FONT_UI, 18),
        (80, 145),
        1760,
        MUTED,
        1.4,
    )

    base = load_logo("lockup-black.png")

    def stretched():
        return base.resize((int(base.width * 1.45), int(base.height * 0.72)), Image.Resampling.LANCZOS)

    def squashed():
        return base.resize((int(base.width * 0.62), int(base.height * 1.35)), Image.Resampling.LANCZOS)

    def rotated():
        return base.rotate(16, expand=True, resample=Image.Resampling.BICUBIC)

    def shadow():
        sh = Image.new("RGBA", (base.width + 40, base.height + 40), (0, 0, 0, 0))
        blob = Image.new("RGBA", base.size, (0, 0, 0, 0))
        blob.paste((0, 0, 0, 90), mask=base.split()[-1])
        blob = blob.filter(ImageFilter.GaussianBlur(8))
        sh.alpha_composite(blob, (18, 16))
        sh.alpha_composite(base, (8, 4))
        return sh

    w0, h0 = base.size
    spined = Image.new("RGBA", (220, 220), (0, 0, 0, 0))
    sd = ImageDraw.Draw(spined)
    t, gap, x0, y0, bw = 28, 18, 40, 30, 140
    for i in range(3):
        yy = y0 + i * (t + gap)
        sd.rectangle((x0, yy, x0 + bw, yy + t), fill=BLACK + (255,))
    sd.rectangle((x0, y0, x0 + t, y0 + 2 * (t + gap) + t), fill=BLACK + (255,))

    donts = [
        (stretched(), "Yatay germe"),
        (squashed(), "Dikey ezme"),
        (rotated(), "Döndürme"),
        (_recolor(base, TERRACOTTA), "Palet dışı renk"),
        (base.crop((0, 0, w0, int(h0 * 0.42))), "Kilit kırma / tagline silme"),
        (shadow(), "Gölge ve efekt"),
        (spined, "E’ye omurga ekleme"),
        (render_e_bars_only(), "Üç çubuğu tek başına kullanma"),
    ]
    gap, cols = 20, 4
    tw = (1760 - gap * 3) // 4
    th = 250
    ox, oy = 80, 220
    for i, (im, label) in enumerate(donts):
        c, r = i % cols, i // cols
        x = ox + c * (tw + gap)
        y = oy + r * (th + 92)
        rounded_rect(d, (x, y, x + tw, y + th), 6, fill=WHITE)
        paste_box(img, im, (x + 18, y + 18, x + tw - 18, y + th - 18))
        d.ellipse((x + tw - 38, y + 12, x + tw - 12, y + 38), fill=(180, 40, 36))
        d.text((x + tw - 31, y + 14), "✕", font=font(FONT_UI_BOLD, 14), fill=WHITE)
        d.text((x, y + th + 12), label, font=font(FONT_UI, 16), fill=CHARCOAL)
    footer(img, 7)
    return img


def slide_08() -> Image.Image:
    img = new_slide(CREAM)
    d = draw(img)
    kicker(d, "Ezgi Craft Renk Paleti", 80, 56)
    heading(d, "Siyah, beyaz, keten", 80, 92, 44)
    draw_wrapped(
        d,
        "Logo yalnızca siyah ve beyazdır. Karışım yok, metalik yok, ara ton yok. "
        "Bu, malzemenin kendisine bırakılan bir boşluktur: renk ahşaptan, kumaştan, mekândan gelir.",
        font(FONT_UI, 20),
        (80, 170),
        860,
        CHARCOAL,
        1.48,
    )
    draw_wrapped(
        d,
        "Dijital yüzeylerde keten krem, terracotta ve adaçayı sayfa ve düğmede yaşar. "
        "Bu renkler logonun içine girmez. Altın, zanaat metalidir; vurgu olarak kalır, kilit olmaz.",
        font(FONT_UI, 20),
        (980, 170),
        860,
        MUTED,
        1.48,
    )

    swatches = [
        ("Siyah", "#111111", BLACK, "Wordmark, ızgara", "0 / 0 / 0 / 93"),
        ("Beyaz", "#FFFFFF", WHITE, "Negatif, boşluk", "—"),
        ("Keten", "#FAF6F0", CREAM, "Sayfa zemini", "—"),
        ("Terracotta", "#C2541A", TERRACOTTA, "Dijital CTA", "—"),
        ("Adaçayı", "#4A6351", SAGE, "İkincil yüzey", "—"),
        ("Altın", "#C5A26B", GOLD, "Yüzey vurgusu, logo değil", "24 / 34 / 65 / 4"),
    ]
    gap = 18
    tw = (1760 - gap * 5) // 6
    ox, oy = 80, 430
    for i, (name, hexv, rgb, role, cmyk) in enumerate(swatches):
        x = ox + i * (tw + gap)
        border = LINE if rgb in (WHITE, CREAM, GOLD) else None
        rounded_rect(d, (x, oy, x + tw, oy + 280), 8, fill=rgb, outline=border, width=1)
        label_c = CREAM if rgb in (BLACK, TERRACOTTA, SAGE) else CHARCOAL
        d.text((x + 16, oy + 200), name, font=font(FONT_UI_BOLD, 16), fill=label_c)
        d.text((x + 16, oy + 226), hexv, font=font(FONT_UI, 14), fill=label_c)
        d.text((x, oy + 300), role, font=font(FONT_UI, 14), fill=MUTED)
        d.text((x, oy + 322), f"CMYK  {cmyk}", font=font(FONT_UI, 13), fill=MUTED)

    bars = [(WHITE, 0.45), (BLACK, 0.25), (CREAM, 0.15), (TERRACOTTA, 0.08), (SAGE, 0.07)]
    cx, by, bw = 80, 830, 1760
    for rgb, pct in bars:
        wdt = int(bw * pct)
        d.rectangle((cx, by, cx + wdt - 4, by + 16), fill=rgb)
        if rgb == WHITE:
            d.rectangle((cx, by, cx + wdt - 4, by + 16), outline=LINE)
        cx += wdt
    footer(img, 8)
    return img


def slide_09() -> Image.Image:
    img = new_slide(CREAM)
    d = draw(img)
    kicker(d, "Ezgi Craft Tipografisi", 80, 56)
    heading(d, "Geometrik kilit, sans nefes", 80, 92, 42)
    draw_wrapped(
        d,
        "Logo, geometrik bir grotesk üzerine kurulur. E ve A özel kesimdir: üç çubuk, açık iskelet. "
        "Kalan harfler Century Gothic ailesindendir. Sıkı değil, ölçülü bir izleme (tracking) logoya "
        "mimari bir ritim verir.",
        font(FONT_UI, 19),
        (80, 170),
        860,
        CHARCOAL,
        1.48,
    )
    draw_wrapped(
        d,
        "Arayüz gövdesi Plus Jakarta Sans’tır; logo ile yarışmaz. Başlıklarda aynı geometrik sans "
        "kullanılabilir. Serif, logonun içine girmez.",
        font(FONT_UI, 19),
        (980, 170),
        860,
        MUTED,
        1.48,
    )

    card(img, (80, 400, 920, 920), WHITE, 8)
    d.text((120, 430), "Logo tipografisi", font=font(FONT_UI_BOLD, 14), fill=MUTED)
    d.text((120, 470), "EZGİ", font=font(FONT_DISPLAY, 56), fill=CHARCOAL)
    d.text((120, 545), "CRAFT", font=font(FONT_DISPLAY_REG, 28), fill=CHARCOAL)
    d.line((120, 600, 860, 600), fill=LINE, width=1)
    d.text((120, 624), "Font", font=font(FONT_UI, 14), fill=MUTED)
    d.text((120, 650), "Century Gothic Bold / Regular", font=font(FONT_UI_BOLD, 20), fill=CHARCOAL)
    d.text((120, 710), "Aa Bb Cc Dd Ee Ff Gg Hh Ii", font=font(FONT_DISPLAY, 24), fill=CHARCOAL)
    d.text((120, 760), "0123456789", font=font(FONT_DISPLAY, 24), fill=CHARCOAL)
    d.text((120, 830), "Özel kesim: E = üç çubuk, A = açık iskelet, İ = noktalı gövde.", font=font(FONT_UI, 15), fill=MUTED)

    card(img, (960, 400, 1840, 920), WHITE, 8)
    d.text((1000, 430), "Metin fontu", font=font(FONT_UI_BOLD, 14), fill=MUTED)
    d.text((1000, 470), "Plus Jakarta Sans", font=font(FONT_UI_BOLD, 34), fill=CHARCOAL)
    d.text((1000, 524), "Light  /  Regular  /  Medium  /  Bold", font=font(FONT_UI, 18), fill=MUTED)
    d.line((1000, 570, 1780, 570), fill=LINE, width=1)
    draw_wrapped(
        d,
        "Doğal ahşabın sıcaklığını, el emeğinin kusursuzluğunu ve minimalist estetiği yaşam alanlarınıza taşıyoruz.",
        font(FONT_UI, 18),
        (1000, 610),
        760,
        MUTED,
        1.5,
    )
    d.text((1000, 840), "Web ve arayüz gövdesi — logo ile yarışmaz.", font=font(FONT_UI, 15), fill=MUTED)
    footer(img, 9)
    return img


def slide_10() -> Image.Image:
    img = new_slide(BLACK)
    d = draw(img)
    kicker(d, "İlk Bakış  ·  ©2026 Ezgi Craft", 80, 56, dark=True)
    d.text((80, 92), "App icon, EZGİ’dir", font=font(FONT_DISPLAY, 40), fill=CREAM)
    draw_wrapped(
        d,
        "Üç çubuk asla tek başına durmaz — hamburger menüye benzer, kullanıcı menü açacak sanır. "
        "Kare ikonda EZGİ wordmark’ı kalır: isim okunur, damga nettir.",
        font(FONT_UI, 20),
        (80, 170),
        820,
        (180, 176, 170),
        1.5,
    )

    px, py, pw, ph = 1120, 160, 340, 720
    rounded_rect(d, (px, py, px + pw, py + ph), 42, fill=(28, 28, 26), outline=(70, 68, 64), width=3)
    d.rounded_rectangle((px + 110, py + 14, px + 230, py + 34), 10, fill=BLACK)
    d.text((px + 28, py + 44), "9:41", font=font(FONT_UI, 13), fill=CREAM)

    icon = Image.new("RGBA", (168, 168), (0, 0, 0, 0))
    idr = ImageDraw.Draw(icon)
    idr.rounded_rectangle((0, 0, 168, 168), 36, fill=BLACK)
    mark = fit(load_logo("e-white.png"), 132, 132)
    icon.alpha_composite(mark, ((168 - mark.width) // 2, (168 - mark.height) // 2))
    img.alpha_composite(icon, (px + (pw - 168) // 2, py + 170))
    d.text((px + 90, py + 370), "Ezgi Craft", font=font(FONT_UI_BOLD, 18), fill=CREAM)
    d.text((px + 118, py + 400), "Atölye", font=font(FONT_UI, 14), fill=(180, 176, 170))

    marks = [
        (load_logo("e-white.png"), BLACK, "EZGİ beyaz"),
        (icon, None, "App icon"),
        (load_logo("e-black.png"), WHITE, "EZGİ siyah"),
    ]
    my = 360
    for im, bg, label in marks:
        if bg is not None:
            rounded_rect(d, (80, my, 200, my + 120), 20, fill=bg)
            paste_box(img, im, (100, my + 16, 180, my + 104))
        else:
            fitted = fit(im, 120, 120)
            img.alpha_composite(fitted, (80, my))
        d.text((220, my + 44), label, font=font(FONT_UI, 18), fill=CREAM)
        my += 150
    footer(img, 10, dark=True)
    return img


def slide_11() -> Image.Image:
    img = new_slide(CREAM)
    d = draw(img)
    kicker(d, "Sosyal Medya Kullanımı  ·  ©2026 Ezgi Craft", 80, 48)
    heading(d, "Profil damga, içerik ızgara", 80, 80, 40)
    draw_wrapped(
        d,
        "Profil görselinde EZGİ wordmark’ı kare veya daire kapsayıcısıyla kullanılır. "
        "Üç çubuk tek başına bırakılmaz. İçerik şablonlarında dikey kilit tekrar eder.",
        font(FONT_UI, 18),
        (80, 145),
        1760,
        MUTED,
        1.4,
    )

    card(img, (80, 230, 820, 920), WHITE, 10)
    d2 = draw(img)
    av = Image.new("RGBA", (92, 92), (0, 0, 0, 0))
    ImageDraw.Draw(av).ellipse((0, 0, 92, 92), fill=BLACK)
    em = fit(load_logo("e-white.png"), 72, 40)
    av.alpha_composite(em, ((92 - em.width) // 2, (92 - em.height) // 2))
    img.alpha_composite(av, (120, 270))
    d2.text((232, 278), "ezgicraft", font=font(FONT_UI_BOLD, 22), fill=CHARCOAL)
    d2.text((232, 312), "Takip Et    Mesaj", font=font(FONT_UI, 14), fill=MUTED)
    for i, (n, l) in enumerate([("48", "gönderi"), ("12,4B", "takipçi"), ("86", "takip")]):
        sx = 130 + i * 220
        d2.text((sx, 390), n, font=font(FONT_UI_BOLD, 20), fill=CHARCOAL)
        d2.text((sx, 418), l, font=font(FONT_UI, 14), fill=MUTED)
    d2.text((120, 470), "Ezgi Craft", font=font(FONT_UI_BOLD, 18), fill=CHARCOAL)
    draw_wrapped(d2, "Furniture & objects. El yapımı ahşap. EST. 1997.", font(FONT_UI, 16), (120, 504), 640, MUTED, 1.4)
    d2.text((120, 575), "ezgicraft.com", font=font(FONT_UI, 15), fill=CHARCOAL)

    for i, (hbg, path) in enumerate([(BLACK, "e-white.png"), (WHITE, "e-black.png"), (BLACK, "e-white.png")]):
        cx = 150 + i * 90
        d2.ellipse((cx, 620, cx + 70, 690), fill=hbg, outline=BLACK, width=2)
        paste_box(img, load_logo(path), (cx + 14, 634, cx + 56, 676))

    photos = [
        photo("collection", 280, 280, darken=0.16),
        photo("material", 280, 280, darken=0.18),
        photo("atelier", 280, 280, darken=0.28),
    ]
    labels = ["Koleksiyon", "Malzeme", "Atölye"]
    for i, (ph, lab) in enumerate(zip(photos, labels)):
        x = 860 + i * 300
        paste_rounded(img, ph, (x, 230), 0)
        overlay = Image.new("RGBA", (280, 70), (17, 17, 17, 150))
        img.alpha_composite(overlay, (x, 440))
        d.text((x + 16, 458), lab, font=font(FONT_UI_BOLD, 16), fill=CREAM)
        ty = 540
        rounded_rect(d, (x, ty, x + 280, ty + 340), 0, fill=BLACK if i == 1 else WHITE)
        if i == 0:
            paste_box(img, load_logo("lockup-black.png"), (x + 24, ty + 40, x + 256, ty + 260))
            d.text((x + 16, ty + 300), "Şablon · açık", font=font(FONT_UI, 13), fill=MUTED)
        elif i == 1:
            paste_box(img, load_logo("lockup-white.png"), (x + 24, ty + 40, x + 256, ty + 260))
            d.text((x + 16, ty + 300), "Şablon · koyu", font=font(FONT_UI, 13), fill=(180, 176, 170))
        else:
            paste_box(img, load_logo("e-black.png"), (x + 24, ty + 80, x + 256, ty + 220))
            d.text((x + 16, ty + 300), "Hikâye / profil", font=font(FONT_UI, 13), fill=MUTED)
    footer(img, 11)
    return img


def slide_12() -> Image.Image:
    img = new_slide(CREAM)
    d = draw(img)
    kicker(d, "Basılı ve Fiziksel Kullanım  ·  ©2026 Ezgi Craft", 80, 48)
    heading(d, "Tek parça, aynı oran", 80, 80, 40)
    draw_wrapped(
        d,
        "Fiziksel uygulamalarda logo her zaman aynı oranlarda ve tek parça olarak yerleştirilir.",
        font(FONT_UI, 18),
        (80, 145),
        1760,
        MUTED,
        1.4,
    )

    card(img, (80, 210, 620, 500), BLACK, 6)
    paste_box(img, load_logo("lockup-white.png"), (130, 240, 570, 450))
    d.text((120, 460), "Kartvizit — ön", font=font(FONT_UI, 14), fill=(180, 176, 170))

    card(img, (650, 210, 1190, 500), WHITE, 6)
    d.text((690, 250), "Ayşe Yılmaz", font=font(FONT_DISPLAY, 26), fill=CHARCOAL)
    d.text((690, 295), "Kurucu  /  Atölye", font=font(FONT_UI, 16), fill=MUTED)
    d.line((690, 340, 1120, 340), fill=BLACK, width=1)
    d.text((690, 365), "hello@ezgicraft.com", font=font(FONT_UI, 15), fill=MUTED)
    d.text((690, 392), "+90 539 892 29 74", font=font(FONT_UI, 15), fill=MUTED)
    d.text((690, 455), "Kartvizit — arka", font=font(FONT_UI, 14), fill=MUTED)

    card(img, (1220, 210, 1840, 500), SAND, 6)
    paste_box(img, load_logo("lockup-black.png"), (1280, 230, 1780, 450))
    d.text((1260, 460), "Askı etiketi", font=font(FONT_UI, 14), fill=MUTED)

    items = [
        (BLACK, "lockup-white.png", "Sticker"),
        (WHITE, "e-black.png", "Laptop sticker"),
        (SAND, "lockup-black.png", "Tote çanta"),
        (BLACK, "stamp-white.png", "Ambalaj damgası"),
    ]
    gap = 20
    tw = (1760 - gap * 3) // 4
    for i, (bg, path, label) in enumerate(items):
        x = 80 + i * (tw + gap)
        y = 540
        rounded_rect(d, (x, y, x + tw, y + 320), 6, fill=bg)
        paste_box(img, load_logo(path), (x + 24, y + 36, x + tw - 24, y + 250))
        lc = CREAM if bg == BLACK else MUTED
        d.text((x + 20, y + 270), label, font=font(FONT_UI, 15), fill=lc)
    footer(img, 12)
    return img


def slide_13() -> Image.Image:
    img = new_slide(CREAM)
    d = draw(img)
    kicker(d, "Dijital Yüzey  ·  ©2026 Ezgi Craft", 80, 40)
    heading(d, "Web’de aynı kilit", 80, 70, 40)

    bx, by, bw, bh = 80, 150, 1760, 830
    rounded_rect(d, (bx, by, bx + bw, by + bh), 12, fill=WHITE, outline=LINE, width=1)
    d.rectangle((bx, by, bx + bw, by + 48), fill=SAND)
    for i, col in enumerate([(224, 108, 96), (232, 196, 96), (120, 186, 120)]):
        d.ellipse((bx + 18 + i * 18, by + 16, bx + 32 + i * 18, by + 30), fill=col)
    d.rounded_rectangle((bx + 120, by + 12, bx + 620, by + 36), 8, fill=WHITE)
    d.text((bx + 136, by + 14), "https://www.ezgicraft.com", font=font(FONT_UI, 13), fill=MUTED)

    hero = photo("hero", bw, bh - 48, darken=0.28)
    img.paste(hero, (bx, by + 48))
    veil = Image.new("RGBA", (bw, bh - 48), (17, 15, 13, 70))
    img.alpha_composite(veil, (bx, by + 48))

    d2 = draw(img)
    paste_box(img, load_logo("lockup-white.png"), (bx + 40, by + 64, bx + 280, by + 260))
    nav = font(FONT_UI, 14)
    for i, item in enumerate(["Ana Sayfa", "Hakkımızda", "Koleksiyon", "İletişim"]):
        d2.text((bx + 1100 + i * 160, by + 88), item, font=nav, fill=CREAM)

    d2.text((bx + 80, by + 360), "TASARLAYAN VE ÜRETEN BİZİZ", font=font(FONT_UI_BOLD, 13), fill=TERRACOTTA)
    d2.text((bx + 80, by + 400), "Yaşam Alanınızı", font=font(FONT_DISPLAY, 52), fill=CREAM)
    d2.text((bx + 80, by + 468), "Sadeleştirin.", font=font(FONT_DISPLAY, 52), fill=CREAM)
    draw_wrapped(
        d2,
        "Doğal ahşabın sıcaklığını, el emeğinin kusursuzluğunu ve minimalist estetiği yaşam alanlarınıza taşıyoruz.",
        font(FONT_UI, 18),
        (bx + 80, by + 555),
        720,
        (230, 220, 205),
        1.45,
    )
    rounded_rect(d2, (bx + 80, by + 660, bx + 300, by + 710), 4, fill=TERRACOTTA)
    d2.text((bx + 108, by + 672), "Koleksiyonu Keşfet", font=font(FONT_UI_BOLD, 15), fill=WHITE)
    footer(img, 13)
    return img


def slide_14() -> Image.Image:
    img = new_slide(WHITE)
    d = draw(img)
    d.rectangle((0, 0, 12, H), fill=BLACK)
    d.text((100, 200), "Teşekkürler,", font=font(FONT_DISPLAY, 52), fill=CHARCOAL)
    draw_wrapped(
        d,
        "Umarız tasarım sürecinden ve tasarımlardan keyif alırsınız. Geri dönüşlerinizi bekliyoruz.",
        font(FONT_UI, 24),
        (100, 300),
        1100,
        MUTED,
        1.5,
    )
    d.text((100, 430), "Sevgiler,", font=font(FONT_UI, 22), fill=CHARCOAL)
    paste_box(img, load_logo("lockup-black.png"), (80, 520, 620, 920))
    footer(img, 14)
    return img


def to_rgb(im: Image.Image) -> Image.Image:
    bg = Image.new("RGB", im.size, CREAM)
    bg.paste(im, mask=im.split()[-1])
    return bg


def main() -> None:
    print("Exporting lockup pack…")
    export_pack(LOCKUP_DIR)
    builders = [
        slide_01, slide_02, slide_03, slide_04, slide_05, slide_06, slide_07,
        slide_08, slide_09, slide_10, slide_11, slide_12, slide_13, slide_14,
    ]
    pages = [to_rgb(fn()) for fn in builders]
    preview_dir = OUT_DIR / "presentation_preview"
    preview_dir.mkdir(parents=True, exist_ok=True)
    for i, p in enumerate(pages, 1):
        p.save(preview_dir / f"slide_{i:02d}.jpg", "JPEG", quality=90, optimize=True)

    pdf_path = OUT_DIR / "Ezgi_Craft_Logo_Presentation.pdf"
    pages[0].save(pdf_path, "PDF", resolution=150.0, save_all=True, append_images=pages[1:])
    desktop_pdf = DESKTOP / "Ezgi_Craft_Logo_Presentation.pdf"
    desktop_pdf.write_bytes(pdf_path.read_bytes())
    print(f"Wrote {pdf_path}")
    print(f"Wrote {desktop_pdf}")


if __name__ == "__main__":
    main()
