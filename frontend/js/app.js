const questionEl = document.getElementById('question');
const resultEl = document.getElementById('result');
const buttons =  document.querySelectorAll('.btn-group button[data-answer]');
const btnGroup = document.querySelector('.btn-group');
const startButton = document.getElementById('start-button');
const restartButton = document.querySelector('#restart-button');
const newObjectInput = document.getElementById('new-object-name');
const addObjectButton = document.getElementById('add-object-button');



let sessionId = null;


function generateSessionId() {
  // Можно использовать простую генерацию случайной строки
  return 'sess-' + Math.random().toString(36).substr(2, 9);
}
function disableButtons() {
  buttons.forEach(btn => {
      btn.disabled = true;           // заблокировать кнопки
      btn.style.display = 'none';    // скрыть кнопки
    });
}

function enableButtons() {
  buttons.forEach(btn => {
      btn.disabled = false;           // заблокировать кнопки
      btn.style.display = 'block';    // скрыть кнопки
    });
}



// Общая обработка ответа
function handleResponse(data) {
  if (data.sessionId) sessionId = data.sessionId;

  const msg = data.message;

  if (!msg) {
    questionEl.textContent = 'Не удалось загрузить вопрос';
    return;
  }

  if (msg.startsWith("game(")) {
    const gameName = msg.slice(5, -1); // вырезаем game(...)
    questionEl.textContent = 'Игра завершена!';
    resultEl.textContent = `Это: ${gameName}`;
    disableButtons();
    restartButton.style.display = 'inline-block';
    restartButton.disabled = false; // показать кнопку "Начать заново"
  } else if (msg.toLowerCase().includes('please enter the name')) {
    questionEl.textContent = 'Объект не найден. Введите имя нового объекта (не реализовано)';
    disableButtons();
     // Показать поле ввода и кнопку "Добавить объект"
    newObjectInput.style.display = 'inline-block';
    newObjectInput.disabled = false;
    addObjectButton.style.display = 'inline-block';
    addObjectButton.disabled = false;

  } 
  else if (msg.startsWith("Game '")) {
    questionEl.textContent = msg;
    disableButtons();
    newObjectInput.style.display = 'none';
    newObjectInput.disabled = true;
    addObjectButton.style.display = 'none';
    addObjectButton.disabled = true;
     // Показать поле ввода и кнопку "Добавить объект"
    restartButton.style.display = 'inline-block';
    restartButton.disabled = false; // показать кнопку "Начать заново"

  }
  else {
    questionEl.textContent = msg;
    resultEl.textContent = '';
    btnGroup.style.display = 'flex';      // показать кнопки ответов
    restartButton.style.display = 'none'; // скрыть кнопку "Начать заново"
    enableButtons();
  }
}


async function sendAnswer(answer) {
  resultEl.textContent = '';
  questionEl.textContent = 'Загрузка...';

  try {
    const response = await fetch('http://localhost:5220/api/akinator/next', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        sessionId: sessionId,
        answer: answer
      })
    });

    if (!response.ok) throw new Error('Ошибка сервера');

    const data = await response.json();
    handleResponse(data);
  } catch (e) {
    questionEl.textContent = 'Ошибка соединения с сервером';
    console.error(e);
  }
}

async function startGame() {
  resultEl.textContent = '';
  buttons.forEach(btn => btn.disabled = false);
  questionEl.style.display = 'block';
  btnGroup.style.display = 'flex';
  startButton.style.display = 'none';
  questionEl.textContent = 'Загрузка вопроса...';
  sessionId = generateSessionId(); 
  enableButtons();

  try {
    const response = await fetch('http://localhost:5220/api/akinator/next', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        sessionId: sessionId,
        answer: ""
      })
    });

    if (!response.ok) throw new Error('Ошибка сервера');

    const data = await response.json();
    handleResponse(data);
  } catch (e) {
    questionEl.textContent = 'Ошибка соединения с сервером';
    console.error(e);
  }
}



// Кнопки ответов
buttons.forEach(btn => {
  btn.addEventListener('click', () => {
    sendAnswer(btn.dataset.answer);
  });
});

// Кнопка "Начать игру"
startButton.addEventListener('click', startGame);
restartButton.addEventListener('click', startGame);
addObjectButton.addEventListener('click', () => {
  const answer = newObjectInput.value.trim();  // value — а не ariaValueMax
  sendAnswer(answer);
});


