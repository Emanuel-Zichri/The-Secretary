src="https://code.jquery.com/jquery-3.6.0.min.js"
src="/ajaxCall.js"


function loadQuote() {
    const data = JSON.parse(localStorage.getItem('installationData')) || {};
    const { customerDetails = {}, spaceDetails = [] } = data;

    let totalSize = 0;
    spaceDetails.forEach(s => totalSize += Number(s.size));

    let pricePerM = 0;
    const parquetType = localStorage.getItem('ParquetType') || '';

    if (parquetType.includes('עץ')) pricePerM = 70;
    else if (parquetType.includes('למינציה')) pricePerM = 40;
    else if (parquetType.includes('פישבון')) pricePerM = 90;

    const estimatedPrice = totalSize * pricePerM;
    const low = Math.round(estimatedPrice * 0.8);
    const high = Math.round(estimatedPrice * 1.2);

    const installDays = Math.ceil(totalSize / 20); // נניח 20 מ"ר ביום

    document.getElementById('summary-list').innerHTML = `
      <li>${totalSize} מ"ר פרקט ${parquetType} - כולל התקנה</li>
      <li>${spaceDetails.length} חללים</li>
    `;
    document.getElementById('priceRange').innerText = `${low} - ${high} ש"ח`;
    document.getElementById('durationRange').innerText = `${installDays - 1} - ${installDays + 1} ימים`;
  }

  function goNext() {
    window.location.href = 'InstallDateSelection.html';
  }

  window.onload = loadQuote;