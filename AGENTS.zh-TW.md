# AGENTS 指南 for dotnet-webapi-lab

## 最高指導原則
本 repo 以 DDD 與 CA / Hex Arch 為最高指導原則，專案與資料夾的命名詞彙必須在這三種設計、架構模式中找到對應詞彙。

## 專案結構
- 方案檔 (`dotnet-lab.slnx`) 位於根目錄。
- **src** 資料夾包含所有 C# 專案。
- **deploy/** 資料夾存放部署相關檔案 (Docker and Helm charts)。
- **keycloak/** 資料夾存放認證範例。
- **.github/workflows/** 資料夾存放 CI 設定。

## 專案結構與分層
本解決方案遵循 Clean Architecture 和 Domain-Driven Design 的原則。專案結構反映了這一點，將關注點明確分離到以下幾個層次：

- **ArchPrecept 套件**：領域基礎型別來自 `ArchPrecept.Contracts.Domain`；聚合儲存庫、查詢與事件分派契約來自 `ArchPrecept.Contracts.Application`，不在本 repo 重建重複契約。
- **`dotnetLab.Domain`**: 這是應用程式的核心。它包含模型化業務領域的領域實體、值物件、聚合和領域事件。它僅依賴於 `ArchPrecept.Contracts.Domain`。
- **`dotnetLab.Application`**: 此層包含應用程式邏輯。它協調領域物件以執行特定的使用案例。它包含應用程式服務、命令、查詢和資料傳輸物件 (DTO)。它依賴於 `Domain` 層。
- **`dotnetLab.Infrastructure`**: 實作在 `Application` 和 `Domain` 層中定義的介面。它處理外部問題，如檔案系統、外部 API 或訊息佇列。
- **`dotnetLab.Persistence.*`**: 此層負責資料持久化。它包含在 `Domain` 層中定義的儲存庫介面的實作，通常使用像 Entity Framework Core 這樣的 ORM。
- **`dotnetLab.WebApi`**: 展示層。它將應用程式的功能公開為 RESTful API。它包含處理 HTTP 請求、將工作委派給 `Application` 層並返回回應的控制器。
- **`dotnetLab.GrpcService`**: 透過 gRPC 服務公開功能的替代展示層。
- **`dotnetLab.CrossCutting.Observability`**: 包含可應用於所有層的橫切關注點，如日誌記錄、指標和追蹤。
- **`ArchPrecept.Analyzers.Core`**：由 `src/Directory.Build.targets` 將 0.0.3-preview 套用至所有正式專案；`src/archprecept.repository.xml` 保留既有架構 Profile 並選用 `ddd`。DBA1001-DBA1017 設為 error，DBA1018／DBA1019 設為 warning。套件領域事件 Port 不需 suppression；DBA0002 的 not-evaluated 代表覆蓋限制，不能當成通過。
- **`dotnetLab.Tests`**：領域、應用層與程序內 HTTP 回歸測試；所有 C# 專案皆放在 `src/`。

## 開發指南
- 維持分層界線：內層不得依賴外層。
- 新增專案時，維持專案命名與組織的一致性。
- 遵循 `.editorconfig` 強制的程式碼風格，commit 前執行 `dotnet format`。
- 確保解決方案可建置：`dotnet build dotnet-lab.slnx -c Release`。
- 建置成功後必須執行單元測試：`dotnet test`。

### 編碼規則
- 非同步方法必須以 `Async` 結尾，並提供接受 `CancellationToken` 的多載。框架 override 保留要求的方法名稱（例如 gRPC `SayHello`），並傳遞框架提供的取消權杖。
- 公開類別與方法必須包含以臺灣繁體中文或英文撰寫的 XML 文件註解。

### Web API 規則
- 表示 API 輸入的型別應以 `Request` 為後綴。
- 表示 API 輸出的型別應以 `ViewModel` 為後綴。
- 每個 Web API 端點都必須指定 `ProducesResponseType<ApiResponse<TResponse>>`。
