#!/usr/bin/env python3
"""Ezgi Craft — brand asset pack from the stacked geometric lockup."""

from __future__ import annotations

import shutil
import struct
import sys
from io import BytesIO
from pathlib import Path

from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "tools"))

from ezgi_lockup import (  # noqa: E402
    INK,
    export_pack,
    fit_height,
    recolor,
    render_app_icon,
    render_ezgi_wordmark,
    render_favicon,
    render_lockup,
    render_social,
    render_stamp,
    save_png,
)

WWWROOT = ROOT / "src" / "ezgi_mobilya.Web" / "wwwroot"
BRAND = ROOT / "brand_assets"
WEB_OUT = WWWROOT / "images" / "logo_assets"

KEEP_BRAND = {
    "lockup",
    "presentation_preview",
    "Ezgi_Craft_Logo_Presentation.pdf",
    "README.md",
    "brand_assets.zip",
}


def write_ico(images: list[Image.Image], path: Path) -> None:
    entries = []
    payloads = []
    for im in images:
        rgba = im.convert("RGBA")
        buf = BytesIO()
        rgba.save(buf, format="PNG")
        data = buf.getvalue()
        payloads.append(data)
        w, h = rgba.size
        entries.append((w if w < 256 else 0, h if h < 256 else 0, len(data)))

    offset = 6 + 16 * len(entries)
    parts = [struct.pack("<HHH", 0, 1, len(entries))]
    cur = offset
    for (w, h, size), _ in zip(entries, payloads):
        parts.append(struct.pack("<BBBBHHII", w, h, 0, 0, 1, 32, size, cur))
        cur += size
    for data in payloads:
        parts.append(data)
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(b"".join(parts))


def clean_brand() -> None:
    for child in BRAND.iterdir():
        if child.name in KEEP_BRAND:
            continue
        if child.is_dir():
            shutil.rmtree(child)
        else:
            child.unlink()


def clean_web() -> None:
    if WEB_OUT.exists():
        shutil.rmtree(WEB_OUT)
    WEB_OUT.mkdir(parents=True, exist_ok=True)


def scale_widths(im: Image.Image, widths: tuple[int, ...]) -> dict[int, Image.Image]:
    out: dict[int, Image.Image] = {}
    for w in widths:
        h = max(1, int(im.height * (w / im.width)))
        out[w] = im.resize((w, h), Image.Resampling.LANCZOS)
    return out


def copy_into(src: Path, dest: Path) -> None:
    dest.parent.mkdir(parents=True, exist_ok=True)
    shutil.copy2(src, dest)


def main() -> None:
    print("Cleaning old gold/Garamond assets…")
    clean_brand()
    clean_web()

    print("Masters…")
    lockup_dir = BRAND / "lockup"
    export_pack(lockup_dir)

    black = render_lockup(INK, width=4000)
    white = recolor(black, (255, 255, 255))
    compact_b = render_lockup(INK, width=2400, compact=True)
    compact_w = recolor(compact_b, (255, 255, 255))
    word_b = render_ezgi_wordmark(INK)
    word_w = recolor(word_b, (255, 255, 255))
    app_dark = render_app_icon(1024, dark=True)
    app_light = render_app_icon(1024, dark=False)
    stamp_b = render_stamp(INK, 1024)
    stamp_w = render_stamp((255, 255, 255), 1024)

    print("Primary lockups…")
    for w, im in scale_widths(black, (500, 1000, 2000, 4000)).items():
        save_png(im, BRAND / "primary" / f"logo-black-{w}.png")
    for w, im in scale_widths(white, (500, 1000, 2000, 4000)).items():
        save_png(im, BRAND / "primary" / f"logo-white-{w}.png")

    print("Compact / wordmark / icon / stamp…")
    save_png(compact_b, BRAND / "compact" / "logo-compact-black.png")
    save_png(compact_w, BRAND / "compact" / "logo-compact-white.png")
    save_png(word_b, BRAND / "wordmark" / "ezgi-black.png")
    save_png(word_w, BRAND / "wordmark" / "ezgi-white.png")
    save_png(app_dark, BRAND / "icon" / "app-icon-1024.png")
    save_png(app_light, BRAND / "icon" / "app-icon-light-1024.png")
    copy_into(lockup_dir / "e-black.png", BRAND / "icon" / "ezgi-mark-black.png")
    copy_into(lockup_dir / "e-white.png", BRAND / "icon" / "ezgi-mark-white.png")
    save_png(stamp_b, BRAND / "stamp" / "stamp-black.png")
    save_png(stamp_w, BRAND / "stamp" / "stamp-white.png")
    copy_into(lockup_dir / "lockup-on-white.png", BRAND / "negatif" / "logo-on-white.png")
    copy_into(lockup_dir / "lockup-on-black.png", BRAND / "negatif" / "logo-on-black.png")

    print("Print…")
    save_png(black.resize((3600, max(1, int(black.height * 3600 / black.width))), Image.Resampling.LANCZOS), BRAND / "print" / "logo-black-300dpi.png")
    save_png(white.resize((3600, max(1, int(white.height * 3600 / white.width))), Image.Resampling.LANCZOS), BRAND / "print" / "logo-white-300dpi.png")
    save_png(stamp_b.resize((1800, 1800), Image.Resampling.LANCZOS), BRAND / "print" / "stamp-black-300dpi.png")
    save_png(stamp_w.resize((1800, 1800), Image.Resampling.LANCZOS), BRAND / "print" / "stamp-white-300dpi.png")

    print("Web favicons / PWA…")
    for size in (16, 32, 48, 64, 128, 256):
        save_png(render_favicon(size), BRAND / "web" / f"favicon-{size}.png")

    save_png(render_app_icon(180, dark=True), BRAND / "web" / "apple-touch-icon.png")
    save_png(render_app_icon(192, dark=True), BRAND / "web" / "android-chrome-192.png")
    save_png(render_app_icon(512, dark=True), BRAND / "web" / "android-chrome-512.png")

    ico_imgs = [render_favicon(s) for s in (16, 32, 48, 64)]
    write_ico(ico_imgs, BRAND / "web" / "favicon.ico")

    print("Social…")
    for name, size in {
        "instagram-110.png": 110,
        "facebook-180.png": 180,
        "linkedin-400.png": 400,
        "twitter-400.png": 400,
        "youtube-800.png": 800,
    }.items():
        save_png(render_social(size), BRAND / "social" / name)

    print("Web layer (yalnızca sitede kullanılan dosyalar)…")
    nav_dark = fit_height(word_b, 104, pad=8)
    nav_light = fit_height(word_w, 104, pad=8)
    save_png(nav_dark, WEB_OUT / "logo_nav.png")
    save_png(nav_light, WEB_OUT / "logo_nav_white.png")

    for name in (
        "favicon.ico",
        "favicon-16.png",
        "favicon-32.png",
        "apple-touch-icon.png",
        "android-chrome-192.png",
        "android-chrome-512.png",
    ):
        copy_into(BRAND / "web" / name, WEB_OUT / name)

    copy_into(BRAND / "web" / "favicon.ico", WWWROOT / "favicon.ico")
    copy_into(BRAND / "web" / "apple-touch-icon.png", WWWROOT / "apple-touch-icon.png")

    print("Nav size:", nav_dark.size)
    print("Brand kit ->", BRAND)
    print("Web subset ->", WEB_OUT)


if __name__ == "__main__":
    main()
