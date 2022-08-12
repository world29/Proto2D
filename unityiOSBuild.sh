#!/bin/bash

UNITY_APP_PATH="/Applications/Unity/Hub/Editor/2020.3.30f1/Unity.app/Contents/MacOS/Unity"
UNITY_PROJECT_PATH="./"
UNITY_BUILDE_NAME="Builder.iOSBuild"
UNITY_LOG_PATH="./Build/iOS/build.log"
PROJECT_DIR="./Build/iOS"

$UNITY_APP_PATH -batchmode \
    -quit \
    -projectPath $UNITY_PROJECT_PATH \
    -executeMethod $UNITY_BUILDE_NAME \
    -logfile $UNITY_LOG_PATH \
    -output-dir $PROJECT_DIR

if [ $? -eq 1 ]; then
    echo "error!! check logfile: ${UNITY_LOG_PATH}"
    exit 1
fi

echo "unity build success!!"

PROJECT_DIR=$PROJECT_DIR sh xcodeBuild.sh
