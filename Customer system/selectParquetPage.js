src="https://code.jquery.com/jquery-3.6.0.min.js"
src="/ajaxCall.js"


const parquetData = {
    solid: [
      { name: 'פרקט עץ אגוז טבעי', img: 'etzMaleEgozTivi.jpg', price: '30', info: 'פרקט איכותי בגוון חום כהה, עמיד במיוחד.' },
      { name: 'פרקט עץ אלון טבעי', img: 'etzMaleAlonTivi.jpg', price: '30', info: 'פרקט אלון הוא אחד הסוגים הפופולריים ביותר של פרקטים בזכות מראהו האלגנטי ועמידותו לאורך זמן.' }
    ],
    laminate: [
      { name: 'פרקט למינציה אפור', img: 'lam1.jpg', price: '20', info: 'פרקט בגוון אפור מודרני, מתאים לעיצוב תעשייתי.' }
    ],
    fishbone: [
      { name: 'פרקט פישבון אלון', img: 'fish1.jpg', price: '35', info: 'עיצוב קלאסי יוקרתי שמתאים לחללים פתוחים.' }
    ]
  };

  function selectTab(tabEl, type) {
    document.querySelectorAll('.tab').forEach(tab => tab.classList.remove('active'));
    tabEl.classList.add('active');
    document.getElementById('parquet-title').innerText = 'פרקטי ' + tabEl.innerText;
    const container = document.getElementById('parquet-options');
    container.innerHTML = parquetData[type].map(item => `
        <div class="parquet-item">
          <div style="position:relative">
            <img src="/picturs/parquetsTypes/${item.img}" alt="${item.name}" />
            <div class="info-btn" onclick="alert('${item.info}')">i</div>
          </div>
          <div style="margin-top: 8px; font-weight: 500; font-size: 14px;">${item.name}</div>
          <div class="price">החל מ־${item.price} ש"ח למ"ר</div>
          <div class="parquet-select">
            <input type="radio" name="parquet" value="${item.name}" />
          </div>
        </div>
      `).join('');
      
  }

  function validateSelection() {
  const selected = document.querySelector('input[name="parquet"]:checked');
  if (!selected) {
    alert('אנא בחרו פרקט לפני מעבר לשלב הבא');
    return;
  }

  localStorage.setItem('ParquetType', selected.value);
  window.location.href = 'InstallDetailsPage.html';
}



  function goBack() {
    window.history.back();
  }


  window.onload = () => {
    selectTab(document.querySelector('.tab.active'), 'solid');
  };