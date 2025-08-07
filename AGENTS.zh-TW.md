# AGENTS 指南 for dotnet-webapi-lab

## 最高指導原則
本 repo 以 DDD 與 CA / Hex Arch 為最高指導原則，專案與資料夾的命名詞彙必須在這三種設計、架構模式中找到對應詞彙。

## 專案結構
- **src** 資料夾包含解決方案 (`dotnet-lab.sln`) 及所有 C# 專案。
  - 專案依分層架構組織：`dotnetLab.BuildingBlocks.Domain`, `dotnetLab.Domain`, `dotnetLab.Application`, `dotnetLab.Infrastructure`, `dotnetLab.Persistence.*`, `dotnetLab.WebApi`, `dotnetLab.GrpcService` 等。
- **deploy/** 資料夾存放部署相關檔案 (Docker and Helm charts)。
- **keycloak/** 資料夾存放認證範例。
- **.github/workflows/** 資料夾存放 CI 設定。

## 開發指南
- 維持分層界線：內層不得依賴外層。
- 新增專案時，維持專案命名與組織的一致性。
- 遵循 `.editorconfig` 強制的程式碼風格，commit 前執行 `dotnet format`。
- 確保解決方案可建置：`dotnet build dotnet-lab.sln -c Release`。
- 建置成功後必須執行單元測試：`dotnet test`。

### 編碼規則
- 非同步方法必須以 `Async` 結尾，並提供接受 `CancellationToken` 的多載。
- 公開類別與方法必須包含以臺灣繁體中文或英文撰寫的 XML 文件註解。

### Web API 規則
- 表示 API 輸入的型別應以 `Request` 為後綴。
- 表示 API 輸出的型別應以 `ViewModel` 為後綴。
- 每個 Web API 端點都必須指定 `ProducesResponseType<ApiResponse<TResponse>>`。
