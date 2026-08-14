"""Ezgi Craft lockup — source artwork, not a redraw."""

from __future__ import annotations

from functools import lru_cache
from pathlib import Path

from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "brand_assets" / "lockup" / "source.png"
INK = (17, 17, 17)


@lru_cache(maxsize=1)
def _knockout_source() -> Image.Image:
    if not SOURCE.exists():
        raise FileNotFoundError(f"Missing source lockup: {SOURCE}")
    return _knockout(Image.open(SOURCE))


def _knockout(im: Image.Image, threshold: int = 242) -> Image.Image:
    """White paper → transparent; remaining ink → solid brand black."""
    rgba = im.convert("RGBA")
    px = rgba.load()
    w, h = rgba.size
    for y in range(h):
        for x in range(w):
            r, g, b, _ = px[x, y]
            lum = (r + g + b) / 3
            if lum >= threshold:
                px[x, y] = (0, 0, 0, 0)
            else:
                alpha = int(min(255, (threshold - lum) / threshold * 255 * 1.15))
                px[x, y] = (*INK, max(32, min(255, alpha)))
    return rgba


def trim_alpha(im: Image.Image, padding: int = 0) -> Image.Image:
    bbox = im.getchannel("A").getbbox()
    if not bbox:
        return im
    x0, y0, x1, y1 = bbox
    x0 = max(0, x0 - padding)
    y0 = max(0, y0 - padding)
    x1 = min(im.width, x1 + padding)
    y1 = min(im.height, y1 + padding)
    return im.crop((x0, y0, x1, y1))


def recolor(im: Image.Image, rgb: tuple[int, int, int]) -> Image.Image:
    out = Image.new("RGBA", im.size, (0, 0, 0, 0))
    src = im.convert("RGBA")
    sp, op = src.load(), out.load()
    r, g, b = rgb
    for y in range(src.height):
        for x in range(src.width):
            _, _, _, a = sp[x, y]
            if a:
                op[x, y] = (r, g, b, a)
    return out


def on_bg(im: Image.Image, bg: tuple[int, int, int], pad: float = 0.14) -> Image.Image:
    p = int(max(im.width, im.height) * pad)
    canvas = Image.new("RGBA", (im.width + p * 2, im.height + p * 2), bg + (255,))
    canvas.alpha_composite(im, (p, p))
    return canvas


def save_png(im: Image.Image, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    im.save(path, "PNG", optimize=True)


def _source_ink() -> Image.Image:
    return trim_alpha(_knockout_source().copy(), 0)


def render_lockup(
    fg: tuple[int, int, int] = INK,
    *,
    width: int = 2000,
    pad_ratio: float = 0.0,
    compact: bool = False,
) -> Image.Image:
    ink = _source_ink()
    if compact:
        ink = _compact(ink)
    scale = width / ink.width
    h = max(1, int(ink.height * scale))
    out = ink.resize((width, h), Image.Resampling.LANCZOS)
    if fg != INK:
        out = recolor(out, fg)
    if pad_ratio:
        pad = int(width * pad_ratio)
        canvas = Image.new("RGBA", (out.width + pad * 2, out.height + pad * 2), (0, 0, 0, 0))
        canvas.alpha_composite(out, (pad, pad))
        return canvas
    return out


def _compact(ink: Image.Image) -> Image.Image:
    """Drop the FURNITURE & OBJECTS line; keep EZGİ / CRAFT / EST."""
    # Measured on 1024x544 source, then mapped through trim.
    # Full source bands (absolute):
    # tittle 123-136, EZGİ 148-262, CRAFT 291-339, rule 356-359,
    # tagline 379-395, EST 414-430
    # ink origin on source: (288, 122)
    ox, oy = 288, 122
    knocked = _knockout_source()

    def crop_src(x0, y0, x1, y1):
        return trim_alpha(knocked.crop((x0, y0, x1, y1)), 0)

    top = crop_src(280, 120, 760, 360)  # EZGİ + CRAFT + rule
    est = crop_src(280, 408, 760, 436)
    gap = 18
    canvas = Image.new("RGBA", (top.width, top.height + gap + est.height), (0, 0, 0, 0))
    canvas.alpha_composite(top, (0, 0))
    ex = (top.width - est.width) // 2
    canvas.alpha_composite(est, (ex, top.height + gap))
    return trim_alpha(canvas, 0)


def render_ezgi_wordmark(fg: tuple[int, int, int] = INK) -> Image.Image:
    """Top line only: EZGİ. Never the three bars alone."""
    src = _knockout_source()
    mark = trim_alpha(src.crop((280, 118, 760, 268)), 0)
    if fg != INK:
        mark = recolor(mark, fg)
    return mark


def render_e_bars_only() -> Image.Image:
    """Isolated three-bar E — only for 'don't use' examples."""
    src = _knockout_source()
    return trim_alpha(src.crop((286, 146, 371, 264)), 1)


def render_e_mark(fg: tuple[int, int, int] = INK, size: int = 1024) -> Image.Image:
    """Square app/icon mark: the EZGİ wordmark, not the hamburger bars."""
    mark = render_ezgi_wordmark(fg)
    canvas = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    pad = int(size * 0.16)
    fitted_w = size - pad * 2
    scale = fitted_w / mark.width
    nh = max(1, int(mark.height * scale))
    mark = mark.resize((fitted_w, nh), Image.Resampling.LANCZOS)
    canvas.alpha_composite(mark, (pad, (size - nh) // 2))
    return canvas


def render_stamp(fg: tuple[int, int, int] = (255, 255, 255), size: int = 1024) -> Image.Image:
    """Circular packaging seal with the compact lockup — reads as a stamp, not a menu."""
    canvas = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    d = ImageDraw.Draw(canvas)
    fill = fg + (255,)
    m = int(size * 0.04)
    d.ellipse((m, m, size - m, size - m), outline=fill, width=max(3, size // 70))
    m2 = int(size * 0.10)
    d.ellipse((m2, m2, size - m2, size - m2), outline=fill, width=max(2, size // 120))
    lock = render_lockup(fg, width=int(size * 0.62), compact=True)
    canvas.alpha_composite(
        lock,
        ((size - lock.width) // 2, (size - lock.height) // 2),
    )
    return canvas


def render_app_icon(size: int = 1024, *, dark: bool = True) -> Image.Image:
    """Rounded-square app icon: EZGİ wordmark on solid ground."""
    bg = INK if dark else (255, 255, 255)
    fg = (255, 255, 255) if dark else INK
    canvas = Image.new("RGBA", (size, size), bg + (255,))
    canvas.alpha_composite(render_e_mark(fg, size))
    return canvas


def render_favicon(size: int) -> Image.Image:
    """Browser-tab badge: opaque white square, black letter E.

    EZGİ cannot read at 16–32px. A black disc vanishes on dark Chrome tabs, and
    transparent corners composite as a black box. This is the wordmark's E
    (shorter middle bar) as a high-contrast badge — not an app icon.
    """
    over = 8 if size <= 32 else 4
    s = size * over
    white = (255, 255, 255, 255)
    ink = INK + (255,)
    canvas = Image.new("RGBA", (s, s), white)
    d = ImageDraw.Draw(canvas)

    pad = max(over, int(s * 0.18))
    inner = s - pad * 2
    bar_h = max(over, int(inner * (0.10 if size <= 16 else 0.07)))
    gap = max(over * 2, int(inner * (0.18 if size <= 16 else 0.16)))
    mark_h = 3 * bar_h + 2 * gap
    outer_w = int(inner * 0.72)
    mid_w = int(outer_w * 0.58)
    x0 = (s - outer_w) // 2
    y0 = (s - mark_h) // 2
    d.rectangle((x0, y0, x0 + outer_w - 1, y0 + bar_h - 1), fill=ink)
    y1 = y0 + bar_h + gap
    d.rectangle((x0, y1, x0 + mid_w - 1, y1 + bar_h - 1), fill=ink)
    y2 = y1 + bar_h + gap
    d.rectangle((x0, y2, x0 + outer_w - 1, y2 + bar_h - 1), fill=ink)

    return canvas.resize((size, size), Image.Resampling.LANCZOS)


def render_social(size: int) -> Image.Image:
    """Circular profile: black disc, white EZGİ, thin ring. Not three bars."""
    canvas = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    d = ImageDraw.Draw(canvas)
    d.ellipse((0, 0, size - 1, size - 1), fill=INK + (255,))
    mark = render_ezgi_wordmark((255, 255, 255))
    inset = int(size * 0.16)
    inner_w = size - inset * 2
    nh = max(1, int(mark.height * (inner_w / mark.width)))
    mark = mark.resize((inner_w, nh), Image.Resampling.LANCZOS)
    canvas.alpha_composite(mark, (inset, (size - nh) // 2))
    ring = max(2, size // 45)
    d.ellipse((ring, ring, size - ring - 1, size - ring - 1), outline=(255, 255, 255, 255), width=ring)
    return canvas


def fit_height(im: Image.Image, height: int, pad: int = 0) -> Image.Image:
    ratio = height / im.height
    resized = im.resize((max(1, int(im.width * ratio)), height), Image.Resampling.LANCZOS)
    if not pad:
        return resized
    out = Image.new("RGBA", (resized.width + pad * 2, resized.height + pad * 2), (0, 0, 0, 0))
    out.alpha_composite(resized, (pad, pad))
    return out


def export_pack(out: Path) -> dict[str, Path]:
    out.mkdir(parents=True, exist_ok=True)
    black = render_lockup(INK, width=2000)
    white = recolor(black, (255, 255, 255))
    compact_b = render_lockup(INK, width=1600, compact=True)
    compact_w = recolor(compact_b, (255, 255, 255))
    e_b = render_e_mark(INK, 1024)
    e_w = render_e_mark((255, 255, 255), 1024)
    stamp_w = render_stamp((255, 255, 255), 1024)
    stamp_b = render_stamp(INK, 1024)

    paths = {
        "lockup-black": out / "lockup-black.png",
        "lockup-white": out / "lockup-white.png",
        "lockup-compact-black": out / "lockup-compact-black.png",
        "lockup-compact-white": out / "lockup-compact-white.png",
        "e-black": out / "e-black.png",
        "e-white": out / "e-white.png",
        "stamp-black": out / "stamp-black.png",
        "stamp-white": out / "stamp-white.png",
        "lockup-on-white": out / "lockup-on-white.png",
        "lockup-on-black": out / "lockup-on-black.png",
    }
    save_png(black, paths["lockup-black"])
    save_png(white, paths["lockup-white"])
    save_png(compact_b, paths["lockup-compact-black"])
    save_png(compact_w, paths["lockup-compact-white"])
    save_png(e_b, paths["e-black"])
    save_png(e_w, paths["e-white"])
    save_png(stamp_b, paths["stamp-black"])
    save_png(stamp_w, paths["stamp-white"])
    save_png(on_bg(black, (255, 255, 255), 0.14), paths["lockup-on-white"])
    save_png(on_bg(white, (17, 17, 17), 0.14), paths["lockup-on-black"])
    return paths


if __name__ == "__main__":
    export_pack(ROOT / "brand_assets" / "lockup")
    preview = on_bg(render_lockup(INK, width=1200), (255, 255, 255), 0.08)
    out = ROOT / "brand_assets" / "presentation_preview" / "_new_lockup.jpg"
    preview.convert("RGB").save(out, quality=95)
    print("ok", preview.size)
