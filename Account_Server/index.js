const express = require('express');
const app = express();

const accountRouter = require('./routers/account');

app.use(express.json({
    inflate: true,
    limit: '10mb'
}));

app.use(express.urlencoded({ extended: true }));
app.use(accountRouter);

const server = app.listen(13200, () => {
    var host = server.address().address;
    var port = server.address().port;

    console.log("账号服务启动，访问地址为 http://%s:%s", host, port);
});