#!/bin/bash
project_id="web.dnc.api.wechat"
version="0.0.1"

docker version
docker build -t guardstudio/${project_id}:${version} .
docker run -d -P -v /var/data/${project_id}:/app/data --name="${project_id}" guardstudio/${project_id}:${version}