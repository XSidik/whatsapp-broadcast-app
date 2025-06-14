const express = require('express');
const { Client, LocalAuth } = require('whatsapp-web.js');
const QRCode = require('qrcode');
require('dotenv').config();

const app = express();
app.use(express.json());

const client = new Client({
    authStrategy: new LocalAuth()
});

let currentQr = null;

client.on('qr', async qr => {
    try {
        const base64 = await QRCode.toDataURL(qr);
        currentQr = base64;
        console.log("New QR generated.");
    } catch (err) {
        console.error("Failed to generate QR code", err);
    }
});

client.on('ready', () => {
    console.log('WhatsApp Client is ready!');
});

client.initialize();

app.get('/qr', (req, res) => {
    if (!currentQr) {
        return res.status(404).send({ error: 'QR not available' });
    }
    res.send({ qr: currentQr });
});

// Broadcast endpoint
app.post('/send', async (req, res) => {
    const { numbers, message } = req.body;

    try {
        for (const number of numbers) {
            await client.sendMessage(number + "@c.us", message);
        }
        res.status(200).send({ status: "Messages sent!" });
    } catch (error) {
        console.error(error);
        res.status(500).send({ error: "Failed to send message." });
    }
});

app.listen(3000, () => console.log('WhatsApp API listening on port 3000'));
