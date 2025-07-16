# Giai đoạn build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy toàn bộ source code
COPY . .

# Di chuyển vào project Web
WORKDIR /app/QuizIT.Web

# Publish ứng dụng ra thư mục /out
RUN dotnet publish -c Release -o /out

# Giai đoạn runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy kết quả build từ stage build vào runtime
COPY --from=build /out ./

# Render yêu cầu chạy cổng 10000
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Entrypoint chạy đúng file .dll
ENTRYPOINT ["dotnet", "QuizIT.Web.dll"]
