if [ -n "$1" ]; then
    echo "归档平台: osx"
    echo "归档版本号: $1"
    build_dir="/root/webserver/BiangStudio/wp-content/themes/BiangTheme/Projects/MechStorm/Build/$1"
    echo "输出路径: $build_dir"
    if [ ! -d "$build_dir" ];then
      mkdir $build_dir
      echo "路径不存在，自动创建"
    fi
    zip_name="$build_dir/MechStorm-MacOS-V$1.app.zip"
    echo "归档压缩包名称: $zip_name"
    if [ ! -f "$zip_name" ];then
      zip -r $zip_name /root/GameServer/MechStorm/Client/Build/osx/*
    else
      echo "已存在该版本zip，取消归档"
    fi
else
    echo "请使用版本号作第一参数"
fi
