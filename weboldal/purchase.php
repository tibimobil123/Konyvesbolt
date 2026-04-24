<?php
$conn = new mysqli("localhost", "root", "", "library");

if ($conn->connect_error) {
    die("Hiba");
}

$book_id = $_POST['book_id'];
$user = $_POST['user'];

$sql = "INSERT INTO purchases (book_id, user_name) VALUES ('$book_id', '$user')";

if ($conn->query($sql)) {
    echo "Megvásárolva!";
} else {
    echo "Hiba!";
}
?>