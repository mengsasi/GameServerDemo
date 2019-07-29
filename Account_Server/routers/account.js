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
    }
});

router.post('/refreshToken', (req, res) => {
    const body = req.body;
    const rtoken = body.token;
    const code = rtoken.substr(0, rtoken.length - 3);
    if (accountDb[code]) {
        //账号存在，返回token，其实应该刷新，省掉了
        res.json({ r: 1, token: rtoken });
    }
    else {
        //账号不存在，重新登陆
        res.json({ r: 0 });
    }
});

module.exports = router;