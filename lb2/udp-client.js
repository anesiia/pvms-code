const dgram = require('dgram');
const readline = require('readline');

const client = dgram.createSocket('udp4');
const SERVER_PORT = 8000;
const SERVER_HOST = '192.168.0.183';

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

rl.question('Enter message for UDP server: ', (message) => {
    const msgBuffer = Buffer.from(message);
    client.send(msgBuffer, 0, msgBuffer.length, SERVER_PORT, SERVER_HOST, (err) => {
        if (err) {
            console.error('Send error:', err);
            client.close();
        }
    });
});

client.on('message', (msg, rinfo) => {
    console.log(`Server response: ${msg.toString()}`);
    client.close();
    rl.close();
});
