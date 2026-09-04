# Repository Guidelines

## Project Structure & Module Organization

This repository contains a Windows WhatsApp desktop client with two applications:

- `wa-frontend/` is the React 19 + TypeScript Vite interface. Application pages are in `src/pages/`, reusable UI in `src/components/`, API clients in `src/api/`, state stores in `src/stores/`, and shared types/utilities in `src/types/` and `src/lib/`. Static images belong in `public/images/`.
- `wa-desktop/WaDesktop/` is the .NET Framework 4.7.2 WinForms host. Code is grouped by workflow under `Features/`; each feature keeps its views, presenters, contracts, models, and data access together. Shared contracts live in `Core/`, cross-feature HTTP plumbing in `Infrastructure/`, navigation in `Shell/`, and dependency registration in `Composition/`.
- `wa-desktop/tests/WaDesktop.Tests/` contains NUnit presenter tests. `build-local.ps1` produces a release package.

## Build, Test, and Development Commands

Run frontend commands from `wa-frontend/`:

```powershell
npm ci                     # install locked dependencies
npm run dev                # start the Vite development server
npm run build              # type-check and create a web build
npm run build:desktop      # write the web build into the WinForms wwwroot folder
```

Build the desktop solution from `wa-desktop/` with:

```powershell
msbuild WaDesktop.sln /p:Configuration=Debug /p:Platform="Any CPU"
```

For a release package, run `./build-local.ps1 -Version 1.2.3` from the repository root; it installs/restores required tooling, builds both apps, and invokes Velopack.

## Coding Style & Naming Conventions

Use TypeScript with two-space indentation, `PascalCase` React component filenames (for example, `ChatWindow.tsx`), and `camelCase` hooks, helpers, and variables (for example, `useMessages`). Prefer Tailwind utility classes for frontend styling and keep API/type changes aligned across `src/api/` and `src/types/`.

Use C# `PascalCase` for public types and members, `I`-prefixed interfaces, and keep WinForms designer changes paired with their `.Designer.cs` and `.resx` files. No formatter or linter script is configured; follow nearby code and ensure `npm run build` succeeds.

## Testing Guidelines

Add NUnit tests under the matching feature in `wa-desktop/tests/WaDesktop.Tests/`, naming files `*Tests.cs` and tests after observable behavior. Run them with `dotnet test wa-desktop/WaDesktop.sln` or Visual Studio Test Explorer. Manually smoke-test affected frontend flows in `npm run dev`; no frontend test runner is currently configured.

## Commit & Pull Request Guidelines

Follow the established Conventional Commit style: `feat(chat-actions): add request_id` or `refactor: align presenters`. Keep commits focused. Pull requests should explain user-visible behavior, link the relevant issue when available, list verification performed, and include screenshots for UI changes. Do not commit generated `wwwroot` output, secrets, or local release artifacts.
