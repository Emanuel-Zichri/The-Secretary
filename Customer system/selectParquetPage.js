let parquetData = {
    solid: [],
    laminate: [],
    fishbone: []
  };
  
  function fetchParquetTypes() {
    ajaxCall(
      "GET",
      `${API_BASE_URL}/ParquetType/GetAll`,
      null,
      function (types) {
        console.log("תוצאה מהשרת:", types); // בדיקת מבנה המידע
  
        types.forEach(type => {
          if (type.typeName.includes("עץ")) parquetData.solid.push(type);
          else if (type.typeName.includes("למינציה")) parquetData.laminate.push(type);
          else if (type.typeName.includes("פישבון")) parquetData.fishbone.push(type);
        });
  
        selectTab($('.tab.active')[0], 'solid'); // טען את הטאב הראשון כברירת מחדל
      },
      function (xhr, status, error) {
        console.error("שגיאה בטעינת סוגי פרקטים:", error);
        alert("אירעה שגיאה בטעינת סוגי הפרקטים מהשרת.");
      }
    );
  }
  
  function selectTab(tabEl, type) {
    $('.tab').removeClass('active');
    $(tabEl).addClass('active');
  
    $('#parquet-title').text('פרקטי ' + $(tabEl).text());
  
    const items = parquetData[type];
    const container = $('#parquet-options');
    container.empty();
  
    items.forEach(item => {
      container.append(`
        <div class="parquet-item">
          <div style="position:relative">
            <img src="/picturs/parquetsTypes/${item.imageURL}" alt="${item.typeName}" />
            <div class="info-btn" onclick='showInfo(${JSON.stringify(item)})'>i</div>

          </div>
          <div style="margin-top: 8px; font-weight: 500; font-size: 14px;">${item.typeName}</div>
          <div class="price">החל מ־${item.pricePerUnit} ש"ח למ"ר</div>
          <div class="parquet-select">
            <input type="radio" name="parquet" value="${item.typeName}" />
          </div>
        </div>
      `);
    });
  }
  
  function escapeQuotes(text) {
    return text.replace(/"/g, '&quot;').replace(/'/g, '&#39;');
  }
  
  function showInfo(item) {
    document.getElementById("info-title").innerText = item.typeName;
    document.getElementById("info-description").innerText = item.description || "אין תיאור זמין.";
    document.getElementById("info-image").src = item.imageURL || "/picturs/default.jpg";
    document.getElementById("info-modal").style.display = "flex";
  }
  
  function closeInfoModal() {
    document.getElementById("info-modal").style.display = "none";
  }
  
  
  function validateSelection() {
    const selected = $('input[name="parquet"]:checked').val();
    if (!selected) {
      alert('אנא בחרו פרקט לפני מעבר לשלב הבא');
      return;
    }
    localStorage.setItem('ParquetType', selected);
    window.location.href = 'InstallDetailsPage.html';
  }
  
  function goBack() {
    window.history.back();
  }
  
  $(document).ready(function () {
    fetchParquetTypes();
  });
  