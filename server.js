const express = require("express");
const bodyParser = require("body-parser");
const db = require("./db");
const cors = require("cors");

const app = express();
app.use(bodyParser.json());
app.use(cors());

// REGISZTRÁCIÓ
app.post("/register", (req, res) => {
    const { email, password } = req.body;
    db.query("INSERT INTO users (email, password) VALUES (?, ?)", [email, password], (err, result) => {
        if(err) return res.status(400).send("Hiba: " + err.message);
        res.send("Sikeres regisztráció");
    });
});

// BEJELENTKEZÉS
app.post("/login", (req, res) => {
    const { email, password } = req.body;
    db.query("SELECT * FROM users WHERE email = ? AND password = ?", [email, password], (err, results) => {
        if(err) return res.status(500).send("Hiba");
        if(results.length === 0) return res.status(401).send("Hibás email vagy jelszó");
        res.send("Sikeres belépés");
    });
});

app.listen(3000, () => console.log("Backend fut a 3000-es porton"));