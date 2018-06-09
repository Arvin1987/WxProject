FROM microsoft/aspnetcore:2.0.5
ENV ASPNETCORE_ENVIRONMENT Production
ENV GUARD_PROJECTID web.dnc.api.wechat
COPY . /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "GuardWebAPI.WeChat.dll"]