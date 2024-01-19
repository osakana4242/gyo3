# 開発メモ

# Unity WebGL を上書きビルドしても最新の状態にならない件

回避方法は `cache busting` を使うこと。URLの末尾にゴミ文字列を追加して、ブラウザがキャッシュを利用するのを回避する手法。

例:

`hoge.js`

`hoge.js?v=1`

`hoge.js?v=2`

# ビルド時に自動で cache busting を追加する

`Assets/WebGLTemplates` 以下に独自のテンプレートを用意する。

PWA をベースにする場合

Unity インストールフォルダ内にテンプレートがあるので、これをコピーする。

例:

`C:/Program Files/Unity/Hub/Editor/2022.3.5f1/Editor/Data/PlaybackEngines/WebGLSupport/BuildTools/WebGLTemplates/PWA`

コピーしたテンプレートを `Assets/WebGLTemplates` 下にペーストする

`Assets/WebGLTemplates/PWA`

フォルダ名は独自のものにリネームする。ここでは custom

`Assets/WebGLTemplates/custom`

Playse Settings の WebGL Template に custom が表示されるので選択する

custom/index.html を開き URL らしき場所に `?v={{{ Date.now() }}}` を付けて回る。※

※. 波括弧の中は JavaScript が記述出来る。ビルド時に解釈される。  
[JavaScript macros](https://docs.unity3d.com/2022.1/Documentation/Manual/webgl-templates.html#:~:text=an%20empty%20string.-,JavaScript%20macros,-JavaScript%20macros%20are)  

編集前

```
          navigator.serviceWorker.register("ServiceWorker.js");

... 省略 ...

      var loaderUrl = buildUrl + "/{{{ LOADER_FILENAME }}}";
      var config = {
        dataUrl: buildUrl + "/{{{ DATA_FILENAME }}}",
        frameworkUrl: buildUrl + "/{{{ FRAMEWORK_FILENAME }}}",
```

編集後

```
          navigator.serviceWorker.register("ServiceWorker.js?v={{{ Date.now() }}}");

... 省略 ...

      var loaderUrl = buildUrl + "/{{{ LOADER_FILENAME }}}?v={{{ Date.now() }}}";
      var config = {
        dataUrl: buildUrl + "/{{{ DATA_FILENAME }}}?v={{{ Date.now() }}}",
        frameworkUrl: buildUrl + "/{{{ FRAMEWORK_FILENAME }}}?v={{{ Date.now() }}}",
```

ビルド後

```
          navigator.serviceWorker.register("ServiceWorker.js?v=1705635864749");

... 省略 ...

      var loaderUrl = buildUrl + "/webgl.loader.js?v=1705635864749";
      var config = {
        dataUrl: buildUrl + "/webgl.data?v=1705635864749",
        frameworkUrl: buildUrl + "/webgl.framework.js?v=1705635864750",
```

# 参考

[プレイヤーへキャッシュクリアさせずにMaximum call stack size exceededを解決する【Unity】](https://whimsicalcat.sakura.ne.jp/%E3%83%97%E3%83%AC%E3%82%A4%E3%83%A4%E3%83%BC%E3%81%B8%E3%82%AD%E3%83%A3%E3%83%83%E3%82%B7%E3%83%A5%E3%82%AF%E3%83%AA%E3%82%A2%E3%82%92%E8%A6%81%E6%B1%82%E3%81%9B%E3%81%9A%E3%81%ABmaximum-call-stack-s/)
