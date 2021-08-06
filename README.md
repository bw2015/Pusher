
## 初始化
> 连接地址
wss://domain/socket.io/?EIO=3&transport=websocket&sid=d373d806-0a6b-4d21-b224-bed7409ba9d1&b64=1
Request
> 2probe

Response
> 3probe
    430 // 连接成功的标识
    [    
        {
        "content":"Ok", 
        "sid":"a13f7313-b81a-406f-b43c-69b6dd821ab0",
        "enableSubscribe":true,
        "enablePublish":false,
        "resultCode":200
        }
    ]

## 心跳
Request
> 5

Response
> 6 // 5+1

## 频道订阅
    421 // 标识ID
    [
    "subscribe",{
        "channel":"44BEC718",   // 频道名
        "sid":"8e3965fd-a814-4412-a00f-104ad78b6ca4"    // 客户端标识
        }
    ]
Response
    431[{"content":"Ok","resultCode":200}]
    
## 消息发送
    42["message",
    "{\"i\":\"40f300551f764649ac0e0eaf30c0efd1\",   // 消息编号
    \"t\":1628267247605,    // 时间戳    
    \"n\":\"E25AFE38\", // 频道名
    \"c\":\"{\\\"MatchID\\\":112902,\\\"BetID\\\":2870474,\\\"ItemID\\\":6511524,\\\"BetMoney\\\":3373.44,\\\"Reward\\\":8095.3997,\\\"Odds\\\":2.3997}\",    // 消息内容
    \"a\":false,\"s\":null,\"nt\":null}"]
