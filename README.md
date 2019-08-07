# GameDemo
服务器demo

&emsp;

## 安装node-v10.16.0-x64

### 1，开启http服务器
双击根目录GameDemo下的starthttp

弹出是否允许访问网络
点确定

&emsp;

### 2，开启Tcp服务器
Unity运行Game_Server项目

&emsp;

### 3，客户端收发包测试
Unity运行Client项目

测试脚本
TestLogin 是http登陆 <br/>
TestGameServer 是tcp登陆

&emsp;

## 注：
如果开另外一个电脑当作服务器
客户端需要配置ip地址
Login.Server_URL = 服务器ip
