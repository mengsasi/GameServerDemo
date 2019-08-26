# GameDemo
服务器demo <br/>
这是一个Unity网络游戏测试模板，可以进行Http登陆，Tcp登陆，收发包 <br/>
也可以自行扩展Tcp包，协议包在Tool文件夹下 <br/>

Account_Server 是http服务器，用node写的 <br/>
Client 是Unity客户端 <br/>
Game_Server 是游戏服务器，为了简单，直接用Unity搭了一个框架 <br/>
Doc 是游戏文档，配置文件夹 <br/>
Database 是存放数据的地方，本来应该用数据库保存，为了简单，方便测试，直接用了文件存储 <br/>
Tool 是工具集 <br/>

&emsp;
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

测试脚本 <br/>
TestLogin 是http登陆 <br/>
TestGameServer 是tcp登陆

&emsp;

## 注：
如果开另外一个电脑当作服务器 <br/>
客户端需要配置ip地址 <br/>
Login.Server_URL = 服务器ip <br/>
TestGameServer NetworkManager.TCPHostUrl = 服务器ip



###待修改
反射消耗性能，可以把所有类型注册起来，用一个int类型的消息号，从字典里取具体类型 <br/>
类似sproto那个c2sprototol <br/>
前后端，消息协议处的代码一致性 <br/>
