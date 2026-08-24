# Backend Scripts

Thu muc nay dung de chua cac script phu tro cho backend, vi du:

- Seed du lieu demo khi can tao data mau rieng cho portfolio/demo.
- Import/export data khi chuyen database.
- Script maintenance chay thu cong.
- Ghi chu lenh deploy/migration rieng cho backend.

Quy uoc:

- Script chay cung app luc startup nen dat trong `Data/Seeders`.
- Script chay thu cong hoac chi dung luc demo/deploy thi dat trong thu muc nay.
- Khong commit file co secret, connection string that, token, service role key.

## Demo seed

Demo data seed chay cung backend khi config duoc bat:

```text
DemoSeed__Enabled=true
```

Khi chay bang Docker Compose, bat bien:

```text
DEMO_SEED_ENABLED=true
```

Seeder co check idempotent: neu da co product SKU bat dau bang `DEMO-` thi se skip, nen build/restart lai khong tao trung data.

Van co the chay script local neu can:

Chay seed demo tren may local:

```powershell
cd Scripts\DemoSeed
dotnet run
```

Mac dinh script doc connection string tu `web_banhang_be/.env`.
Co the truyen connection string truc tiep:

```powershell
dotnet run -- --connection "Host=localhost;Port=5432;Database=WebBanHang;Username=postgres;Password=postgres"
```

Tai khoan demo:

```text
seller.demo@shoppy.local / demo123456
buyer.demo@shoppy.local / demo123456
```
