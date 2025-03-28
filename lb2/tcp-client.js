const net = require('net');
const readline = require('readline');

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});


const client = new net.Socket();
client.connect(8000, '192.168.0.183', () => {
    console.log('Connected to TCP server on 8000');
    rl.question('Enter message for server: ', (message) => {
        client.write(message);
    });
});

client.on('data', (data) => {
    console.log('Server responce: ' + data.toString());
    client.destroy();
});

client.on('close', () => {
    console.log('Connection closed');
});
