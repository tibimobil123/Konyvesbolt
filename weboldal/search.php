<?php
$conn = new mysqli("localhost", "root", "", "library");

if ($conn->connect_error) {
    die("Hiba: " . $conn->connect_error);
}

$keres = $_GET['q'] ?? '';

if (trim($keres) === '') {
    echo json_encode([]);
    exit();
}

$sql = "SELECT * FROM books WHERE title LIKE '%$keres%' OR author LIKE '%$keres%'";
$result = $conn->query($sql);

$books = [];

while($row = $result->fetch_assoc()){
    $books[] = $row;
}

echo json_encode($books);
?>