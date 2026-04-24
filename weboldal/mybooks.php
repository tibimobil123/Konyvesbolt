<?php
$conn = new mysqli("localhost", "root", "", "library");

$user = $_GET['user'] ?? '';

if ($user == '') {
    echo json_encode([]);
    exit();
}

$sql = "SELECT books.title, books.author, books.price, COUNT(*) AS quantity, SUM(books.price) AS total_price
        FROM purchases
        JOIN books ON purchases.book_id = books.id
        WHERE purchases.user_name = '$user'
        GROUP BY books.id, books.title, books.author, books.price";

$result = $conn->query($sql);

$books = [];

while($row = $result->fetch_assoc()){
    $books[] = $row;
}

echo json_encode($books);
?>