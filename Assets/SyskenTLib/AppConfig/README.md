# About

アプリ内で使える定義ファイルです


# Detail

ConfigMasterのAppRootConfigが設定ファイルの親です。
このファイルに紐付いている設定ファイルが使われます。


# How To

1. シーンにAppConfigManager.prefabを追加しておいてください。
2. AppConfigManager.instance.GetCurrentConfig().メソッド名のように定数を取得できます。
