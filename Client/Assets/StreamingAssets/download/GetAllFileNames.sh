#!/bin/bash

rm FileSizeList.txt
function echo_name(){
 for file in `ls $1`
 do
  if [ -d "$1/$file" ];
  then
    echo_name "$1/$file"
  else
   sizeNumber=`size "$1/$file"`
   md5sum=`md5sum "$1/$file" | awk '{print $1}'`
   echo "$sizeNumber-$md5sum-$1/$file"
  fi
 done
}

size() {
  stat -c %s $1 | tr -d '\n'
}

echo_name . >> FileSizeList.txt

