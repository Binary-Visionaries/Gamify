@echo off
"D:\\Unity\\2023.2.5f1\\Editor\\Data\\PlaybackEngines\\AndroidPlayer\\OpenJDK\\bin\\java" ^
  --class-path ^
  "C:\\Users\\Nicro\\.gradle\\caches\\modules-2\\files-2.1\\com.google.prefab\\cli\\2.0.0\\f2702b5ca13df54e3ca92f29d6b403fb6285d8df\\cli-2.0.0-all.jar" ^
  com.google.prefab.cli.AppKt ^
  --build-system ^
  cmake ^
  --platform ^
  android ^
  --abi ^
  arm64-v8a ^
  --os-version ^
  23 ^
  --stl ^
  c++_shared ^
  --ndk-version ^
  23 ^
  --output ^
  "C:\\Users\\Nicro\\Desktop\\Gamify\\Gamify\\.utmp\\RelWithDebInfo\\o701x1v3\\prefab\\arm64-v8a\\prefab-configure" ^
  "C:\\Users\\Nicro\\.gradle\\caches\\transforms-3\\c6da488043b8f3167369588fb03ded40\\transformed\\jetified-games-activity-2.0.2\\prefab" ^
  "C:\\Users\\Nicro\\.gradle\\caches\\transforms-3\\3f4c5dd9f3b16b166b25d4c523b2fcc1\\transformed\\jetified-games-frame-pacing-1.10.0\\prefab"
