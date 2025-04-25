let roomCount = 0;

const roomTypeOptions = [
  "סלון",
  "חדר שינה",
  "מטבח",
  "חדר רחצה",
  "מרפסת",
  "חדר ילדים",
  "מסדרון"
];

function generateRoomTypeSelect() {
  return `
    <select required class="room-type-select" onchange="updateHeader(this)">
      <option value="">בחר סוג חלל</option>
      ${roomTypeOptions.map(type => `<option value="${type}">${type}</option>`).join('')}
    </select>
  `;
}

function addRoom() {
  roomCount++;
  const container = document.getElementById('rooms-container');
  const div = document.createElement('div');
  div.className = 'room-entry';
  div.innerHTML = `
    <div class="room-header">
      <span data-base-label="סוג חלל ${roomCount}">סוג חלל ${roomCount}</span>
      <div>
        <button class="toggle-btn" onclick="toggleRoom(this)">−</button>
        <button class="remove-btn" onclick="removeRoom(this.closest('.room-entry'))">×</button>
      </div>
    </div>
    <div class="room-body">
      <div class="form-group">${generateRoomTypeSelect()}</div>
      <div class="form-group" style="position: relative;">
        <span class="info-btn" onclick="showPopup(event)">i</span>
        <input type="number" placeholder="שטח החלל במ״ר" required />
      </div>
      <div class="form-group">
        <input type="text" placeholder="הערות לחלל (לא חובה)" />
      </div>
      <div class="form-group upload-wrapper">
        <label>
          העלאת סרטון
          <i>↢</i>
          <input type="file" accept="video/*" />
        </label>
      </div>
    </div>`;
  container.appendChild(div);
}

function toggleRoom(btn) {
  const room = btn.closest('.room-entry');
  room.classList.toggle('collapsed');
  btn.textContent = room.classList.contains('collapsed') ? '+' : '−';
}

function updateHeader(input) {
  const header = input.closest('.room-entry').querySelector('.room-header span');
  const baseLabel = header.dataset.baseLabel || header.textContent;
  const value = input.value?.trim?.() || input.value || "";
  header.textContent = value !== "" ? value : baseLabel;
}

function removeRoom(roomDiv) {
  const container = document.getElementById('rooms-container');
  if (container.querySelectorAll('.room-entry').length > 1) {
    roomDiv.remove();
  } else {
    alert("לא ניתן להסיר את החלל האחרון.");
  }
}

function saveAllRooms() {
  const roomDivs = document.querySelectorAll('.room-entry');
  const spaceDetails = [];
  let isValid = true;

  const parquetType = localStorage.getItem('ParquetType');

  roomDivs.forEach((div, index) => {
    const select = div.querySelector('select.room-type-select');
    const inputs = div.querySelectorAll('input');
    const floorType = select?.value?.trim?.() || "";
    const size = parseFloat(inputs[0]?.value.trim());
    const notes = inputs[1]?.value.trim();
    const mediaFile = inputs[2]?.files[0];

    if (!floorType || isNaN(size)) {
      isValid = false;
      div.style.borderColor = 'red';
    } else {
      div.style.borderColor = '#ddd';
      spaceDetails.push({
        spaceID: index,
        requestID: 0,
        size: size,
        floorType: floorType,
        mediaURL: mediaFile ? mediaFile.name : '',
        notes: notes || '',
        ParquetType: parquetType
      });
    }
  });

  if (!isValid) {
    alert("אנא מלא את כל השדות הדרושים לפני ההמשך.");
    return;
  }

  const existingData = localStorage.getItem('installationData');
  const parsed = existingData ? JSON.parse(existingData) : {};
  parsed.spaceDetails = spaceDetails;

  localStorage.setItem('installationData', JSON.stringify(parsed));
  console.log("installationData:", parsed);
  window.location.href = 'customerDetails.html';
}

function showPopup(event) {
  const popup = document.getElementById('popup');
  const rect = event.target.getBoundingClientRect();
  const scrollY = window.scrollY || document.documentElement.scrollTop;
  popup.style.display = 'block';
  popup.style.top = `${rect.top + scrollY}px`;
  popup.style.left = `${rect.right + 10}px`;
}

function closePopup() {
  document.getElementById('popup').style.display = 'none';
}

window.addEventListener('DOMContentLoaded', () => {
  addRoom(); // הוסף חלל ראשון כברירת מחדל
});
