const mysql = require("mysql2");

const db = mysql.createConnection({
    host: "localhost",
    user: "root",
    password: "",
    database: "library"
});

db.connect(err => {
    if(err) console.log("Hiba az adatbázishoz csatlakozáskor:", err);
    else console.log("Csatlakozva az adatbázishoz!");
});

module.exports = db;