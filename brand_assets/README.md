# EZGI CRAFT — Asset Paketi

Kaynak burasıdır. Web katmanı (`wwwroot/images/logo_assets`) yalnızca sitede
kullanılan küçük bir alt kümedir; marka kitinin kopyası değildir.

Dikey geometrik kilit. Logo paleti yalnızca siyah ve beyazdır.
Üç çubuk **asla** hamburger menü, sticker veya app icon olarak tek başına
kullanılmaz. Sekme favicon’u istisnadır: 16 px’te EZGİ okunmaz; dairesel
rozet içindeki kısa orta çubuklu **E** harfidir.

## Renkler

| Renk        | HEX     | RGB           | Rol                                      |
|-------------|---------|---------------|------------------------------------------|
| Siyah       | #111111 | 17, 17, 17    | Wordmark, kilit                          |
| Beyaz       | #FFFFFF | 255, 255, 255 | Negatif, boşluk                          |
| Keten       | #FAF6F0 | 250, 246, 240 | Sayfa zemini (logo içine girmez)         |
| Terracotta  | #C2541A | 194, 84, 26   | Dijital CTA (logo içine girmez)          |
| Adaçayı     | #4A6351 | 74, 99, 81    | İkincil yüzey (logo içine girmez)        |
| Altın       | #C5A26B | 197, 162, 107 | Yüzey vurgusu (logo içine girmez)        |

## Klasörler

- `lockup/source.png` — ana sanat eseri (kaynak)
- `primary/` — tam kilit, şeffaf siyah / beyaz (500–4000 px)
- `compact/` — tagline düşer, EST. 1997 kalır
- `wordmark/` — yalnızca **EZGİ** (navbar kaynağı)
- `icon/` — kare app icon (EZGİ wordmark, menü değil)
- `stamp/` — dairesel ambalaj mühürü
- `negatif/` — beyaz / siyah kapsayıcılı blok
- `web/` — sekme favicon (beyaz kare + siyah E); PWA / apple-touch kare EZGİ
- `social/` — yuvarlak profil görselleri
- `print/` — 300 DPI baskı PNG

## Web katmanı

`src/ezgi_mobilya.Web/wwwroot/images/logo_assets/` içinde yalnızca:

- `logo_nav.png` / `logo_nav_white.png`
- `favicon.ico` / `favicon-16.png` / `favicon-32.png`
- `apple-touch-icon.png`
- `android-chrome-192.png` / `android-chrome-512.png`

## Kullanım

- Oranları bozmayın, uzatmayın.
- Koruma alanı ≈ E çubuğunun kalınlığı.
- Açık zeminde siyah kilit, koyu zeminde beyaz kilit.
- Üç çubuğu hamburger, sticker veya app icon olarak tek başına kullanmayın.
  Sekme favicon’u: beyaz kare içinde kısa orta çubuklu E.

Yeniden üretim: `python tools/generate_logos.py`
