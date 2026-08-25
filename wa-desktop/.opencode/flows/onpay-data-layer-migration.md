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
- [ ] **Fase 3** — Pilot fitur Company end-to-end (payload+interface+datasource+repo+presenter)
- [ ] **Fase 4** — Rollout: Billing → AppSettings → User → Template → Waba → PhoneNumber → Auth
- [ ] **Fase 5** — Hapus IApiClient + ApiService partials lama; update tests; update AGENTS.md

## Checklist WAJIB Per Fitur (tiap commit)

1. [ ] Payload DTO snake_case di `Infrastructure/Payloads/{Feature}/`
2. [ ] Entity dibersihkan dari `[JsonProperty]` (mapping pindah ke Repository)
3. [ ] Interface repo di `Domain/Interfaces/`
4. [ ] DataSource + Repository di `Infrastructure/Data/`
5. [ ] Presenter ganti dependensi + tangani `result.IsSuccess`
6. [ ] Register DI di `ServiceCollectionExtensions`
7. [ ] **Daftarkan semua file .cs baru di .csproj** ← penyebab CS0246 klasik
8. [ ] Update fake test bila presenter tersangkut test
9. [ ] Build + smoke test tab terkait

## Out of Scope

- MessagesView / EmbeddedServer / WebView2 bridge — tidak disentuh
- Velopack update flow — tidak disentuh
- wa-frontend — tidak disentuh

## Verifikasi Akhir (regression manual)

Semua tab MenuBar (Company, Users, Templates, App Settings), klik node TreeView,
logout → login cycle, matikan backend >15 menit → pastikan auto-refresh token.
