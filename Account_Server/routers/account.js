const express = require('express');
let router = express.Router();

//假数据库数据
const fs = require('fs');
const accountDb = JSON.parse(fs.readFileSync('../Database/accountDb.json'));

router.use((req, res, next) => {
    next();
});

router.get('/config-version', (req, res) => {
    res.json({ version: 999 });
});

//下载所有配置，应该将所有配置表，加到一个json文件中，省略了
//只做测试用
router.get('/all-config', (req, res) => {
    const levelConfigs = JSON.parse(fs.readFileSync('../Database/Level.json'));
    res.json({ "Level.json": levelConfigs });
});

//{"code": "123", "sdk": "debug", "version": 999}
router.post('/login', (req, res) => {
    const body = req.body;
    const code = body.code;
    if (body.sdk === 'debug') {
        if (body.version === 999) {
            if (!accountDb[code]) {
                accountDb[code] = {
                    usename: code,
                    lastTimeLogin: new Date()
                };
                fs.writeFileSync('../Database/accountDb.json', JSON.stringify(accountDb));
            }
            var rtoken = code + "lls"
            res.json({ token: rtoken });
        }
        else {
            res.status(500).send('');
        }
    }
});

router.post('/refreshtoken', (req, res) => {
    const body = req.body;
    const rtoken = body.token;
    const code = rtoken.substr(0, rtoken.length - 3);
    if (accountDb[code]) {
        //账号存在，返回token，其实应该刷新，省掉了
        res.json({ r: 0, token: rtoken });
    }
    else {
        //账号不存在，重新登陆
        res.json({ r: 1 });
    }
});

module.exports = router;