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

- **`dotnetLab.BuildingBlocks.Domain`**: 包含領域層的基本建構模塊，例如實體 (`IDomainEntity`)、值物件 (`IValueObject`)、聚合 (`IAggregateRoot`) 和儲存庫 (`IRepository`) 的基礎類別。它不依賴於任何其他專案層。
- **`dotnetLab.Domain`**: 這是應用程式的核心。它包含模型化業務領域的領域實體、值物件、聚合和領域事件。它僅依賴於 `dotnetLab.BuildingBlocks.Domain`。
- **`dotnetLab.Application`**: 此層包含應用程式邏輯。它協調領域物件以執行特定的使用案例。它包含應用程式服務、命令、查詢和資料傳輸物件 (DTO)。它依賴於 `Domain` 層。
- **`dotnetLab.Infrastructure`**: 實作在 `Application` 和 `Domain` 層中定義的介面。它處理外部問題，如檔案系統、外部 API 或訊息佇列。
- **`dotnetLab.Persistence.*`**: 此層負責資料持久化。它包含在 `Domain` 層中定義的儲存庫介面的實作，通常使用像 Entity Framework Core 這樣的 ORM。
- **`dotnetLab.WebApi`**: 展示層。它將應用程式的功能公開為 RESTful API。它包含處理 HTTP 請求、將工作委派給 `Application` 層並返回回應的控制器。
- **`dotnetLab.GrpcService`**: 透過 gRPC 服務公開功能的替代展示層。
- **`dotnetLab.CrossCutting.Observability`**: 包含可應用於所有層的橫切關注點，如日誌記錄、指標和追蹤。
- **`dotnetLab.Analyzers`**: 提供自訂的 C# 原始碼分析器，以強制執行專案特定的編碼標準和最佳實踐。

## 開發指南
- 維持分層界線：內層不得依賴外層。
- 新增專案時，維持專案命名與組織的一致性。
- 遵循 `.editorconfig` 強制的程式碼風格，commit 前執行 `dotnet format`。
- 確保解決方案可建置：`dotnet build dotnet-lab.slnx -c Release`。
- 建置成功後必須執行單元測試：`dotnet test`。

### 編碼規則
- 非同步方法必須以 `Async` 結尾，並提供接受 `CancellationToken` 的多載。
- 公開類別與方法必須包含以臺灣繁體中文或英文撰寫的 XML 文件註解。

### Web API 規則
- 表示 API 輸入的型別應以 `Request` 為後綴。
- 表示 API 輸出的型別應以 `ViewModel` 為後綴。
- 每個 Web API 端點都必須指定 `ProducesResponseType<ApiResponse<TResponse>>`。
