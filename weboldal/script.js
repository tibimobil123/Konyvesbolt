// -------------------- NAVBAR --------------------
const toggleButton = document.getElementsByClassName('navbar-button')[0]
const navbarLinks = document.getElementsByClassName('navbar-links')[0]
toggleButton.addEventListener('click', () => {
  navbarLinks.classList.toggle('active')
});
// -------------------- SEARCH --------------------
document.getElementById("search").addEventListener("input", async (e) => {
  const query = e.target.value;
  const resultsDiv = document.getElementById("results");
  if (query.trim() === "") {
    resultsDiv.innerHTML = "";
    return;
  }

  const res = await fetch(`search.php?q=${query}`);
  const data = await res.json();

  resultsDiv.innerHTML = "";

  data.forEach(book => {
    resultsDiv.innerHTML += `
      <p>
        ${book.title} - ${book.author} - ${book.price} Ft
        <button onclick="purchase(${book.id})">Vásárlás</button>
      </p>
    `;
  });
});

async function loadMyBooks() {
  if (!currentUser) return;
  const res = await fetch("mybooks.php?user=" + currentUser);
  const data = await res.json();

  const div = document.getElementById("mybooks");
  div.innerHTML = "";

  data.forEach(book => {
    let quantityText = book.quantity > 1 ? ` X${book.quantity}` : "";
    div.innerHTML += `<p>${book.title} - ${book.author}${quantityText} - ${book.total_price} Ft</p>`;
  });
}

loadMyBooks();

async function purchase(bookId) {
  if (!currentUser) {
  alert("Csak bejelentkezés után lehetséges!");
  return;
  }
  const res = await fetch("purchase.php", {
    method: "POST",
    headers: {
      "Content-Type": "application/x-www-form-urlencoded"
    },
    body: "book_id=" + bookId + "&user=" + currentUser
  });

  const msg = await res.text();
  alert(msg);
  loadMyBooks();
}
// -------------------- BACKEND --------------------
const overlayLogin = document.getElementById("overlayLogin");
const overlayRegister = document.getElementById("overlayRegister");
const showLoginBtn = document.getElementById("showLogin");
const statusP = document.getElementById("status");
statusP.style.color = "rgb(211, 77, 66)";
let currentUser = null;

// Login modal megnyitás
showLoginBtn.addEventListener("click", () => {
  if(currentUser){
    currentUser = null;
    statusP.textContent = "Kijelentkezve";
    statusP.style.color = "rgb(211, 77, 66)";
    showLoginBtn.textContent = "Belépés/Regisztráció";
    document.getElementById("mybooks").innerHTML = "";

    const modal = document.querySelector("#overlayLogin .modal");
    
    if(!document.getElementById("modalEmail")){
      const emailInput = document.createElement("input");
      emailInput.type = "email";
      emailInput.id = "modalEmail";
      emailInput.placeholder = "Email";
      modal.insertBefore(emailInput, document.getElementById("loginBtn"));
    }
    if(!document.getElementById("modalPassword")){
      const passwordInput = document.createElement("input");
      passwordInput.type = "password";
      passwordInput.id = "modalPassword";
      passwordInput.placeholder = "Jelszó";
      modal.insertBefore(passwordInput, document.getElementById("loginBtn"));
    }
    return;
  }
  overlayLogin.style.display = "flex";

});
// Login modal bezárás X-szel
document.getElementById("closeLogin").addEventListener("click", () => {
  overlayLogin.style.display = "none";
});
// Login → Register
document.getElementById("showRegister").addEventListener("click", () => {
  overlayLogin.style.display = "none";
  overlayRegister.style.display = "flex";
});
// Register → Login vissza
document.getElementById("backToLogin").addEventListener("click", () => {
  overlayRegister.style.display = "none";
  overlayLogin.style.display = "flex";
});

// Login gomb
document.getElementById("loginBtn").addEventListener("click", async () => {
  const emailInput = document.getElementById("modalEmail");
  const passwordInput = document.getElementById("modalPassword");
  const email = emailInput.value;
  const password = passwordInput.value;
  try {
    const res = await fetch("http://localhost:3000/login", {
      method: "POST",
      headers: {"Content-Type":"application/json"},
      body: JSON.stringify({ email, password })
    });
    const text = await res.text();
    alert(text);
    if(res.ok){
      overlayLogin.style.display = "none";
      currentUser = email;
      statusP.textContent = `Bejelentkezve: ${email}`;
      statusP.style.color = "rgb(90, 255, 75)";
      showLoginBtn.textContent = "Kijelentkezés";
      loadMyBooks()
      if(emailInput) emailInput.remove();
      if(passwordInput) passwordInput.remove();
    }
  } catch(err){
    alert("Kapcsolódási hiba a szerverrel");
  }
});

// Register gomb
document.getElementById("registerBtn").addEventListener("click", async () => {
  const email = document.getElementById("registerEmail").value;
  const password = document.getElementById("registerPassword").value;
  try {
    const res = await fetch("http://localhost:3000/register", {
      method: "POST",
      headers: {"Content-Type":"application/json"},
      body: JSON.stringify({ email, password })
    });
    const text = await res.text();
    alert(text);
    if(res.ok) overlayRegister.style.display = "none";
  } catch(err){
    alert("Kapcsolódási hiba a szerverrel");
  }
});