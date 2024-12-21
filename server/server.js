const net = require('net');
const WebSocket = require('ws');

// 创建TCP服务器（接收Unity日志）
const tcpServer = net.createServer();
const port = 3000;

// 创建WebSocket服务器（转发到VSCode）
const wss = new WebSocket.Server({ port: 3001 });

let vscodeConnection = null;

// 处理WebSocket连接
wss.on('connection', (ws) => {
    console.log('VSCode extension connected');
    vscodeConnection = ws;

    ws.on('close', () => {
        console.log('VSCode extension disconnected');
        vscodeConnection = null;
    });
});

// 处理TCP连接
tcpServer.on('connection', (socket) => {
    console.log('Unity client connected');

    let buffer = '';
    socket.on('data', (data) => {
        buffer += data.toString();
        
        // 处理可能的多条日志
        let newlineIndex;
        while ((newlineIndex = buffer.indexOf('\n')) !== -1) {
            const logMessage = buffer.substring(0, newlineIndex);
            buffer = buffer.substring(newlineIndex + 1);
            
            // 转发到VSCode
            if (vscodeConnection) {
                vscodeConnection.send(logMessage);
            }
        }
    });

    socket.on('error', (err) => {
        console.error('Socket error:', err);
    });

    socket.on('close', () => {
        console.log('Unity client disconnected');
    });
});

tcpServer.listen(port, () => {
    console.log(`Server listening on port ${port}`);
}); 