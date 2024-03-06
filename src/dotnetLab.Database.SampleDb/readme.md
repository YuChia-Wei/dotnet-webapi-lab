# database schema for EF Core

給 EFCore 使用的資料庫結構專案，在 Windows 上可以配合 visual studio 中的 EF Core Power Tools 來同步資料庫結構與設定。

如果沒有 EF Core Power Tool 的話，也可以另外靠 EFCore 的 CLI 去跟資料庫同步。

此專案中的 Entities 資料夾不限於給 EFCore 使用，實務上，如果 Dapper 需要直接輸出特定資料表，也可以使用此專案內的 Entities 物件，不用另外建立專用轉接物件。