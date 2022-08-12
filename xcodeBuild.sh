#!/bin/bash

SCHEME="Unity-iPhone"
PROJECT_PATH="${PROJECT_DIR}/${SCHEME}.xcodeproj"
ARCHIVE_FILE="${SCHEME}.xcarchive"
ARCHIVE_DIR="${PROJECT_DIR}/archive"
ARCHIVE_PATH="${ARCHIVE_DIR}/${ARCHIVE_FILE}"
IPA_DIR="${ARCHIVE_DIR}/output_ipa"
EXPORT_OPTIONS_PLIST="Build/ExportOptions.plist"
PROVISIONING_PROFILE="cc14f429-44ea-4702-815f-f3c9a0dd8c94"

mkdir -p $ARCHIVE_PATH

# ARCHIVE
xcodebuild -project $PROJECT_PATH \
    -scheme $SCHEME \
    -destination 'generic/platform=iOS' \
    archive -archivePath $ARCHIVE_PATH \
    PROVISIONING_PROFILE=$PROVISIONING_PROFILE

# ipaファイルの作成
xcodebuild -exportArchive -archivePath $ARCHIVE_PATH \
    -exportPath $IPA_DIR \
    -exportOptionsPlist $EXPORT_OPTIONS_PLIST \
    PROVISIONING_PROFILE=$PROVISIONING_PROFILE
