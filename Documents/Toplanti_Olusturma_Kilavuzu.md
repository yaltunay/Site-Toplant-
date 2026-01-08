# Toplantı Oluşturma Kılavuzu

## Genel Bakış

Bu kılavuz, Toplantı ve Karar Yönetim Sistemi'nde toplantı oluşturma, yönetme ve oylama işlemlerini adım adım açıklamaktadır.

---

## 1. Toplantı Oluşturma

### Adım 1: Toplantı Yönetimi Sekmesine Giriş

1. Ana ekranda **"Toplantı Yönetimi"** sekmesine tıklayın.

### Adım 2: Toplantı Bilgilerini Girme

**"Yeni Toplantı Oluştur"** bölümünde aşağıdaki bilgileri doldurun:

- **Başlık**: Toplantının konusu veya başlığı (örnek: "2024 Yılı Olağan Genel Kurul Toplantısı")
- **Tarih**: Toplantının yapılacağı tarih ve saat (tarih seçici kullanarak)
- **Toplam Arsa Payı**: Site toplam arsa payı değeri (ondalıklı sayı olarak, örnek: 1000.00)

### Adım 3: Toplantıyı Oluşturma

1. Tüm alanları doldurduktan sonra **"Toplantı Oluştur"** butonuna tıklayın.
2. Başarılı mesajı görüntülendiğinde toplantı oluşturulmuş olur.

---

## 2. Toplantıya Katılım Ekleme

### Adım 1: Toplantı Seçme

1. **"Toplantı Yönetimi"** sekmesindeki toplantı listesinden oluşturduğunuz toplantıyı seçin.
2. Seçilen toplantının bilgileri üst bölümde görüntülenir.

### Adım 2: Katılım Ekleme

1. **"Katılım Ekle"** butonuna tıklayın.
2. Açılan pencerede toplantıya katılacak birimi seçin.
3. **"Tamam"** butonuna tıklayın.
4. Birim toplantıya eklenir.

**Not**: Aynı birim iki kez eklenemez. Sistem uyarı verecektir.

---

## 3. Vekalet (Proxy) Yönetimi

### Vekalet Ekleme

1. **"Vekalet Ekle"** butonuna tıklayın.
2. Açılan pencerede:
   - **Vekalet Veren Birim**: Vekalet veren birimi seçin
   - **Vekalet Alan Birim**: Vekalet alan birimi seçin
3. **"Tamam"** butonuna tıklayın.

### Vekalet Kuralları (KMK 31)

- **40 birimden fazla**: Maksimum vekalet sayısı = Toplam birim sayısının %5'i
- **40 birim veya daha az**: Maksimum 2 vekalet

Sistem otomatik olarak bu limitleri kontrol eder ve uyarı verir.

---

## 4. Yeter Sayı (Nisap) Kontrolü

### Yeter Sayı Nedir? (KMK 30)

Toplantının geçerli olması için:
- **Birim Sayısı**: Toplam birim sayısının **%50'sinden fazlası** katılmış olmalı
- **Arsa Payı**: Toplam arsa payının **%50'sinden fazlası** temsil edilmiş olmalı

### Yeter Sayı Kontrolü Yapma

1. Toplantıya katılımları ekledikten sonra **"Yeter Sayı Kontrolü"** butonuna tıklayın.
2. Sistem otomatik olarak kontrol eder ve sonucu gösterir:
   - **Yeşil mesaj**: Yeter sayı sağlandı
   - **Kırmızı mesaj**: Yeter sayı sağlanamadı

**Örnek Mesaj:**
```
Toplantı yeter sayısı sağlanmıştır. 
Birim: 25/50 (%50.0), 
Arsa Payı: 550.00/1000.00 (%55.0)
```

---

## 5. Karar Oluşturma ve Oylama

### Adım 1: Karar Oluşturma

1. **"Oylama"** sekmesine geçin.
2. **"Yeni Karar/Oylama"** bölümünde:
   - **Karar Başlığı**: Kararın başlığını girin
   - **Açıklama**: Kararın detaylı açıklamasını girin
3. **"Karar Oluştur"** butonuna tıklayın.

### Adım 2: Oylama Yapma

1. Oluşturulan kararın yanındaki **"Oyla"** butonuna tıklayın.
2. Açılan pencerede her birim için oy seçin:
   - **Evet**: Kararı destekliyor
   - **Hayır**: Kararı desteklemiyor
   - **Çekimser**: Oy kullanmıyor
3. **"Tamam"** butonuna tıklayın.

### Adım 3: Oylama Sonuçları

Oylama sonuçları otomatik olarak hesaplanır:

- **Birim Sayısı Bazında**: Evet, Hayır, Çekimser oy sayıları
- **Arsa Payı Bazında**: Her seçeneğin toplam arsa payı

**Karar Onay Kriterleri:**
- Evet oyları > Toplam katılan birimlerin %50'si
- Evet arsa payı > Toplam katılan arsa payının %50'si

Karar durumu otomatik olarak **"Kabul"** veya **"Red"** olarak işaretlenir.

---

## 6. Toplantı Tutanağı Oluşturma

### Tutanak Oluşturma

1. Toplantı seçiliyken **"Tutanak Oluştur"** butonuna tıklayın.
2. Sistem otomatik olarak Türkçe tutanak oluşturur.

### Tutanak İçeriği

Tutanak şu bilgileri içerir:

- **Toplantı Bilgileri**: Tarih, konu, açıklama
- **Yeter Sayı (Nisap)**: Birim ve arsa payı detayları
- **KMK 30 Notu**: Yasal uyumluluk notu
- **Vekaletler (KMK 31)**: Vekalet detayları ve limitler
- **Kararlar**: Tüm kararlar ve oylama sonuçları

### Tutanak İşlemleri

- **Kopyala**: Tutanağı panoya kopyalar
- **Kapat**: Pencereyi kapatır

---

## 7. Toplantı Bilgilerini Görüntüleme

### Toplantı Listesi

**"Toplantı Yönetimi"** sekmesindeki tabloda şu bilgiler görüntülenir:

- **Tarih**: Toplantı tarihi ve saati
- **Başlık**: Toplantı başlığı
- **Toplam Birim**: Toplam birim sayısı
- **Katılan Birim**: Toplantıya katılan birim sayısı
- **Yeter Sayı**: Yeter sayının sağlanıp sağlanmadığı (Evet/Hayır)

### Toplantı Detayları

Toplantı seçildiğinde üst bölümde şu bilgiler görüntülenir:

- Toplantı başlığı ve tarihi
- Toplam ve katılan birim sayıları
- Toplam ve katılan arsa payları
- Yeter sayı durumu ve detaylı mesaj

---

## 8. Sık Karşılaşılan Sorunlar ve Çözümleri

### Sorun: "Lütfen önce bir toplantı seçin" Hatası

**Çözüm**: Toplantı listesinden bir toplantı seçin.

### Sorun: "Bu birim zaten toplantıya katılmış" Uyarısı

**Çözüm**: Bir birim aynı toplantıya sadece bir kez eklenebilir.

### Sorun: "Vekalet sayısı limiti aşıldı" Uyarısı

**Çözüm**: KMK 31 kurallarına göre maksimum vekalet sayısına ulaşıldı. Daha fazla vekalet eklenemez.

### Sorun: "Yeter sayı sağlanamadı" Mesajı

**Çözüm**: 
- Daha fazla birim ekleyin
- Vekalet ekleyerek temsil edilen birim sayısını artırın
- Toplam birim sayısının %50'sinden fazlası ve toplam arsa payının %50'sinden fazlası temsil edilmelidir

---

## 9. İpuçları ve En İyi Uygulamalar

### Toplantı Öncesi Hazırlık

1. **Birimleri Hazırlayın**: Tüm birimlerin sisteme ekli olduğundan emin olun
2. **Arsa Paylarını Kontrol Edin**: Toplam arsa payının doğru olduğundan emin olun
3. **Birim Tiplerini Ayarlayın**: Villa, Daire, Dükkan gibi tipleri önceden tanımlayın

### Toplantı Sırasında

1. **Katılımları Sırayla Ekleyin**: Her birimi tek tek ekleyin
2. **Vekaletleri Önceden Planlayın**: Vekalet limitlerini göz önünde bulundurun
3. **Yeter Sayıyı Kontrol Edin**: Her katılım sonrası kontrol yapın

### Toplantı Sonrası

1. **Tutanak Oluşturun**: Toplantı sonunda mutlaka tutanak oluşturun
2. **Tutanakları Kaydedin**: Tutanakları kopyalayıp kaydedin
3. **Kararları Kontrol Edin**: Tüm kararların doğru kaydedildiğinden emin olun

---

## 10. Yasal Uyumluluk

### KMK 30 - Yeter Sayı (Nisap)

- Toplam birim sayısının %50'sinden fazlası
- Toplam arsa payının %50'sinden fazlası

### KMK 31 - Vekalet Kuralları

- 40 birimden fazla: Maksimum %5 vekalet
- 40 birim veya daha az: Maksimum 2 vekalet

Sistem bu kuralları otomatik olarak kontrol eder ve uyarı verir.

---

## 11. Destek ve İletişim

Herhangi bir sorun yaşarsanız veya yardıma ihtiyacınız olursa:

1. Hata mesajlarını not edin
2. Ekran görüntüleri alın
3. Sistem yöneticisi ile iletişime geçin

---

## Ek Notlar

- Tüm tarihler Türkçe formatında gösterilir (gg.aa.yyyy)
- Arsa payları 2 ondalık basamakla gösterilir
- Yeter sayı hesaplamaları otomatik yapılır
- Tutanaklar Türkçe olarak oluşturulur ve yasal gereklilikleri karşılar

---

**Son Güncelleme**: 2024
**Versiyon**: 1.0

