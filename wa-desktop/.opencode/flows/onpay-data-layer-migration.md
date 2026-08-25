# Rencana Migrasi: Struktur Data Layer ala onpay-dekstop (Hybrid)

> Branch: `refactor/onpay-alignment` · Dimulai: 2026-08-25
> Prinsip: MVP Passive View DIPERTAHANKAN. Yang diadopsi dari onpay hanya
> taksonomi folder data-layer, `Result<T,E>`, DelegatingHandler, dan Payloads.

## Keputusan Desain

| Aspek | Keputusan |
|---|---|
| Jumlah assembly | Tetap 3 (Domain, Infrastructure, Client) — boundary dijaga compiler |
| Pola presentasi | Tetap MVP Passive View (bukan smart-component ala onpay) |
| Service locator | TIDAK diadopsi — tetap constructor injection via ModuleFactory |
| UseCase seremonial | Hanya untuk flow ber-orkestrasi (login, phone-registration); CRUD langsung Repository |
| `Result<T, Error>` | Diadopsi, ganti throw/catch exception di boundary presenter |
| Refresh token | Pindah ke `DelegatingHandler`, tapi logika single-flight `SemaphoreSlim` milik wa-desktop (BUKAN poll-flag onpay) |

## Peta Fitur → Repository Baru

| Repository | Method (dari IApiClient) | Konsumen |
|---|---|---|
| `IAuthRepository` | LoginAsync | AuthService |
| `ICompanyRepository` | Get/Create/Update/Delete Company | Company, Users, LimitBilling, Wabas |
| `IBillingRepository` | GetBillingAnalyticsAsync, GetBillingSummaryAsync | Company, Sidebar, Tagihan |
| `IUserRepository` | Users CRUD + ResetPassword | Users |
| `ITemplateRepository` | Get/Sync/Delete Template | Templates, TemplateDetail |
| `IWabaRepository` | Get/Update/Sync Wabas | Wabas, Sidebar, Templates |
| `IPhoneNumberRepository` | 12 method (list/detail/sync/avatar) | PhoneNumbers, PhoneNumberDetail, Sidebar |
| `IAppSettingsRepository` | Settings + SetupWebhook + WebhookStatus | AppSettings |

Registrasi nomor HP (CreatePhoneNumber/RequestCode/VerifyCode/RegisterPhone)
TIDAK dipecah — sudah terbungkus `IPhoneRegistrationUseCase`.

## Struktur Target Per Fitur

```
Domain/Interfaces/I{Feature}Repository.cs        ← kontrak kecil
Infrastructure/Payloads/{Feature}/*.cs           ← DTO snake_case murni
Infrastructure/Data/Remote/DataSources/
    BaseDataSource.cs                            ← unwrap {data,message} → Result
    {Feature}DataSource.cs                       ← HTTP tipis
Infrastructure/Data/Repositories/{Feature}Repository.cs  ← mapping payload→entity
Infrastructure/Data/Remote/Handlers/AuthDelegatingHandler.cs
Infrastructure/Services/AuthSessionStore.cs      ← token state + event SessionExpired
```

## Fase Eksekusi

- [x] **Fase 0** — branch `refactor/onpay-alignment`, baseline commit
- [x] **Fase 1** — `WaDesktop.Domain/Common/{Result,Error}.cs` (C# 7.3-safe)
- [x] **Fase 2** — AuthDelegatingHandler + AuthSessionStore; Core.cs jadi tipis;
      ApiClient facade lama TETAP HIDUP; ShellPresenter re-wire event
      → Catatan deviasi: event TIDAK di-rewire di ShellPresenter; ApiClient
      men-forward event dari store sehingga Program.cs/AuthService tak tersentuh.
      Rewire penuh ditunda ke Fase 5 (lebih aman, nol perubahan konsumen).
      Guard tambahan: Bearer/refresh hanya untuk host API — token tidak bocor
      ke host eksternal (avatar Meta CDN), endpoint auth dikecualikan dari retry
      agar kredensial salah tidak memicu SessionExpired palsu.
- [x] **Fase 3** — Pilot fitur Company end-to-end (payload+interface+datasource+repo+presenter)
      → Catatan deviasi: entity `Company` MASIH membawa `[JsonProperty]` karena
      path legacy (`GetListAsync<Company>` dipakai Users/WabasPresenter) masih
      membaca langsung entity. Pembersihan atribut dilakukan di Fase 5 setelah
      semua fitur pindah stack baru.
      Struktur baru aktif: `ApiHttpPipeline` (HttpClient singleton bersama),
      `BaseDataSource` (Result + unwrap + map status HTTP→ErrorType),
      `CompanyPayload`, `CompanyDataSource`, `CompanyRepository`,
      kontrak `ICompanyRepository`.
      Konsumen baru: CompanyPresenter (penuh), LimitBillingPresenter +
      SidebarPresenter (ctor +1 dependensi).
- [x] **Fase 4** — Rollout: Billing → AppSettings → User → Template → Waba → PhoneNumber → Auth
  Commit: 9792a29 (4a Billing), 6e08892 (4b AppSettings), 0e8af7f (4c User),
  edd74ca (4d Template), 08b5d00 (4e Waba), 845c599 (4f PhoneNumber),
  ad2ff13 (4g Auth + buang IApiClient tak terpakai dari SidebarPresenter).
  Catatan 4f/4g: `BaseDataSource` ditambah `GetBytesAsync` (404→Success(null)
  untuk gambar profil Meta CDN) dan `SendContentAsync` (multipart upload).
- [x] **Fase 5** — Hapus IApiClient + ApiService partials lama; update tests; update AGENTS.md
  Commit: 654bac7. Registration flow pindah ke `IPhoneNumberRepository`
  (UseCase pertahankan semantik throw). `SavePhoneResult`/`AuthResult`
  dipromosikan ke `Domain/Entities`. Bridge Program.cs kini subscribe
  `IAuthSessionStore.SessionExpired/TokenRefreshed` langsung. Tests tidak
  tersentuh (hanya fake IAuthService, tidak ada fake IApiClient).

## Checklist WAJIB Per Fitur (tiap commit)

1. [x] Payload DTO snake_case di `Infrastructure/Payloads/{Feature}/`
2. [ ] Entity dibersihkan dari `[JsonProperty]` (mapping pindah ke Repository) — **satu-satunya sisa**: sebagian besar entity kini hanya diisi lewat mapping manual Repository, tapi `AuthResult` masih jadi tipe payload langsung di `AuthDataSource`; strip atribut = kerjaan kosmetik menyusul
3. [x] Interface repo di `Domain/Interfaces/`
4. [x] DataSource + Repository di `Infrastructure/Data/`
5. [x] Presenter ganti dependensi + tangani `result.IsSuccess`
6. [x] Register DI di `ServiceCollectionExtensions`
7. [x] **Daftarkan semua file .cs baru di .csproj** ← penyebab CS0246 klasik
8. [x] Update fake test bila presenter tersangkut test (tidak ada yang perlu)
9. [ ] Build + smoke test tab terkait ← **BELUM dijalankan sejak fase 3**

## Out of Scope

- MessagesView / EmbeddedServer / WebView2 bridge — tidak disentuh
- Velopack update flow — tidak disentuh
- wa-frontend — tidak disentuh

## Verifikasi Akhir (regression manual)

Semua tab MenuBar (Company, Users, Templates, App Settings), klik node TreeView,
logout → login cycle, matikan backend >15 menit → pastikan auto-refresh token.
