let dataTable1; // ✅ declare it globally
const BASE_PATH = window.BASE_PATH || '/';
const CSRF_TOKEN =
    document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
// Normalize a possibly relative app path to include the PathBase
function toAppUrl(p) {
    if (!p) return '';
    if (/^https?:\/\//i.test(p)) return p; // absolute URL
    p = String(p).replace(/\\/g, '/');
    if (p.startsWith('~/')) p = p.slice(2);
    if (p.startsWith('/')) p = p.slice(1);
    return BASE_PATH + p;
}

let kibarAllItems = [];
let kibarFilteredItems = [];
let kibarPageSize = 12;
let kibarPage = 1;

function safeText(v) {
    if (v === null || v === undefined) return '';
    return String(v);
}

function escapeHtml(str) {
    return safeText(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

function stripHtmlToText(html) {
    const raw = safeText(html);
    if (!raw) return '';
    try {
        const tmp = document.createElement('div');
        tmp.innerHTML = raw;
        const text = tmp.textContent || tmp.innerText || '';
        return text.replace(/\s+/g, ' ').trim();
    } catch {
        return raw.replace(/<[^>]*>/g, ' ').replace(/\s+/g, ' ').trim();
    }
}

function buildTags(tags) {
    const t = safeText(tags).trim();
    if (!t) return '';
    const parts = t.split(',').map(s => s.trim()).filter(Boolean);
    if (!parts.length) return '';
    return parts.map(x => `<span class="badge rounded-pill">${escapeHtml(x)}</span>`).join('');
}

function buildToggle(label, className, id, checked) {
    const c = checked ? 'checked' : '';
    return `
<div class="form-check form-switch">
  <input class="form-check-input ${className}" type="checkbox" data-id="${id}" ${c}>
  <label class="form-check-label small">${escapeHtml(label)}</label>
</div>`;
}

function renderKibarCards(items) {
    const cards = document.getElementById('kibarCards');
    const empty = document.getElementById('kibarCardsEmpty');
    const count = document.getElementById('kibarCount');
    if (!cards) return;

    const total = kibarAllItems.length;
    const shown = items.length;
    if (count) {
        count.textContent = total === shown
            ? `${shown} Einträge`
            : `${shown} von ${total} Einträgen`;
    }

    if (!items.length) {
        cards.innerHTML = '';
        if (empty) empty.classList.remove('d-none');
        renderKibarPagination(0);
        return;
    }

    if (empty) empty.classList.add('d-none');

    const html = items.map(row => {
        const id = row.id;
        const title = escapeHtml(row.title);
        const desc = escapeHtml(stripHtmlToText(row.description));
        const order = escapeHtml(row.verlauf);
        const imgSrc = toAppUrl(row.imageUrl);

        const base = (window.UPSERT_BASE && typeof window.UPSERT_BASE === 'string')
            ? window.UPSERT_BASE
            : (BASE_PATH + 'admin/makerspaceproject/upsert');
        const editHref = `${base}?id=${encodeURIComponent(id)}`;

        const projectUrl = safeText(row.projectUrl).trim();
        const projectHref = projectUrl ? toAppUrl(projectUrl) : '';

        return `
<article class="kibar-item" data-id="${id}">
  <div class="kibar-item-media">
    ${row.imageUrl ? `<img src="${imgSrc}" alt="${title}">` : `<div class="text-muted"><i class="bi bi-image"></i></div>`}
  </div>
  <div class="kibar-item-body">
    <div class="kibar-item-meta">
      <div class="kibar-order"><i class="bi bi-sort-numeric-down"></i> ${order}</div>
      <div class="text-muted small">ID: ${id}</div>
    </div>
    <h3 class="kibar-item-title">${title}</h3>
    <div class="kibar-tags">${buildTags(row.tags)}</div>
    ${desc ? `<p class="kibar-item-desc">${desc}</p>` : ``}
    <div class="kibar-item-toggles">
      ${buildToggle('Tipp', 'lesezeichen-toggle', id, !!row.lesezeichen)}
      ${buildToggle('Forschung', 'forschung-toggle', id, !!row.forschung)}
      ${buildToggle('Lehre', 'top-toggle', id, !!row.top)}
      ${buildToggle('Lernen', 'events-toggle', id, !!row.events)}
      ${buildToggle('Tutorial', 'tutorial-toggle', id, !!row.tutorial)}
      ${buildToggle('IT-Recht', 'ITRecht-toggle', id, !!row.itRecht)}
      ${buildToggle('Beiträge', 'beitraege-toggle', id, !!row.beitraege)}
    </div>
  </div>
  <div class="kibar-item-actions">
    <div>
      ${projectHref ? `<a class="btn btn-sm kibar-open-btn" href="${projectHref}" target="_blank" rel="noopener" title="In neuem Tab öffnen" aria-label="In neuem Tab öffnen"><i class="bi bi-box-arrow-up-right"></i></a>` : ``}
    </div>
    <div class="d-flex align-items-center gap-3 kibar-action-buttons">
      <a href="${editHref}" class="btn btn-sm btn-warning" title="Bearbeiten">
        <i class="bi bi-pencil-square"></i>
      </a>
      <button class="btn btn-sm btn-danger delete-project" data-id="${id}" title="Löschen">
        <i class="bi bi-trash-fill"></i>
      </button>
    </div>
  </div>
</article>`;
    }).join('');

    cards.innerHTML = html;
}

function renderKibarPagination(totalItems) {
    const wrap = document.getElementById('kibarPaginationWrap');
    const ul = document.getElementById('kibarPagination');
    if (!wrap || !ul) return;

    const size = Math.max(1, parseInt(kibarPageSize, 10) || 12);
    const totalPages = Math.max(1, Math.ceil((totalItems || 0) / size));
    kibarPage = Math.min(Math.max(1, kibarPage), totalPages);

    if (totalItems <= size) {
        wrap.classList.add('d-none');
        ul.innerHTML = '';
        return;
    }

    wrap.classList.remove('d-none');

    const makeItem = (labelHtml, page, disabled, active, aria) => {
        const cls = `page-item${disabled ? ' disabled' : ''}${active ? ' active' : ''}`;
        const aCls = 'page-link';
        const attrs = [];
        if (aria) attrs.push(`aria-label="${aria}"`);
        if (active) attrs.push('aria-current="page"');
        return `<li class="${cls}"><a href="#" class="${aCls}" data-page="${page}" ${attrs.join(' ')}>${labelHtml}</a></li>`;
    };

    let html = '';
    html += makeItem('&laquo;', Math.max(1, kibarPage - 1), kibarPage === 1, false, 'Vorherige');

    const windowSize = 2;
    const start = Math.max(1, kibarPage - windowSize);
    const end = Math.min(totalPages, kibarPage + windowSize);

    if (start > 1) {
        html += makeItem('1', 1, false, kibarPage === 1);
        if (start > 2) {
            html += `<li class="page-item disabled"><span class="page-link">…</span></li>`;
        }
    }

    for (let p = start; p <= end; p++) {
        html += makeItem(String(p), p, false, p === kibarPage);
    }

    if (end < totalPages) {
        if (end < totalPages - 1) {
            html += `<li class="page-item disabled"><span class="page-link">…</span></li>`;
        }
        html += makeItem(String(totalPages), totalPages, false, kibarPage === totalPages);
    }

    html += makeItem('&raquo;', Math.min(totalPages, kibarPage + 1), kibarPage === totalPages, false, 'Nächste');
    ul.innerHTML = html;
}

function applyKibarSearch() {
    const q = safeText(document.getElementById('kibarSearch')?.value).trim().toLowerCase();
    if (!q) {
        kibarFilteredItems = [...kibarAllItems];
    } else {
        kibarFilteredItems = kibarAllItems.filter(x => {
            const hay = [x.title, x.tags, x.description].map(safeText).join(' ').toLowerCase();
            return hay.includes(q);
        });
    }

    const size = Math.max(1, parseInt(kibarPageSize, 10) || 12);
    const totalPages = Math.max(1, Math.ceil(kibarFilteredItems.length / size));
    kibarPage = Math.min(Math.max(1, kibarPage), totalPages);
    const start = (kibarPage - 1) * size;
    const pageItems = kibarFilteredItems.slice(start, start + size);

    renderKibarCards(pageItems);
    renderKibarPagination(kibarFilteredItems.length);
}

function loadKibarCards() {
    return $.ajax({
        url: BASE_PATH + 'admin/makerspaceproject/getall',
        type: 'GET',
        datatype: 'json'
    }).then(function (resp) {
        const list = (resp && resp.data) ? resp.data : [];
        kibarAllItems = Array.isArray(list) ? list : [];
        applyKibarSearch();
    }).catch(function () {
        kibarAllItems = [];
        applyKibarSearch();
    });
}

function reloadAfterMutation() {
    return loadKibarCards().finally(function () {
        try {
            if (dataTable1 && dataTable1.ajax) dataTable1.ajax.reload(null, false);
        } catch { }
    });
}

function updateLocalItem(id, patch) {
    const idx = kibarAllItems.findIndex(x => String(x.id) === String(id));
    if (idx >= 0) {
        kibarAllItems[idx] = { ...kibarAllItems[idx], ...patch };
        applyKibarSearch();
    }
}

$(document).ready(function () {

    // Keep old DataTable init as a no-op fallback if DataTables is present.
    try {
        const hasCards = !!document.getElementById('kibarCards');
        if (!hasCards && $.fn && $.fn.DataTable && document.getElementById('tblMakerSpace')) {
            dataTable1 = $('#tblMakerSpace').DataTable({
                "ajax": {
                    url: BASE_PATH + 'admin/makerspaceproject/getall',
                    type: 'GET',
                    datatype: 'json'
                },
                "columns": [
                    { data: 'verlauf', width: "3%" },
                    { data: 'imageUrl', width: "10%" },
                    { data: 'title', width: "20%" },
                    { data: 'lesezeichen', width: "8%" },
                    { data: 'forschung', width: "8%" },
                    { data: 'top', width: "8%" },
                    { data: 'events', width: "8%" },
                    { data: 'tutorial', width: "8%" },
                    { data: 'itRecht', width: "8%" },
                    { data: 'beitraege', width: "8%" },
                    { data: 'id', width: "10%" }
                ],
                "paging": false,
                "searching": false,
                "info": false,
                "ordering": false
            });
        }
    } catch { }

    if (document.getElementById('kibarCards')) {
        loadKibarCards();
    }

    document.getElementById('kibarSearch')?.addEventListener('input', function () {
        kibarPage = 1;
        applyKibarSearch();
    });

    document.getElementById('kibarPageSize')?.addEventListener('change', function (e) {
        const v = parseInt(e.target.value, 10);
        kibarPageSize = Number.isFinite(v) && v > 0 ? v : 12;
        kibarPage = 1;
        applyKibarSearch();
    });

    document.getElementById('kibarPagination')?.addEventListener('click', function (e) {
        const a = e.target?.closest?.('a[data-page]');
        if (!a) return;
        e.preventDefault();
        const p = parseInt(a.getAttribute('data-page'), 10);
        if (!Number.isFinite(p) || p <= 0) return;
        kibarPage = p;
        applyKibarSearch();
    });


    $(document).on('change', '.top-toggle', function () {
        const id = $(this).data('id');
        const isChecked = $(this).is(':checked');

        $.ajax({
            url:  BASE_PATH + 'Admin/MakerSpaceProject/ToggleTop',
            type: 'POST',
            data: { id: id, isTop: isChecked },
            success: function (response) {
                if (!response.success) {
                    Swal.fire('Fehler!', response.message, 'error');
                }
            },
            error: function () {
                Swal.fire('Fehler!', 'Ein Fehler ist aufgetreten.', 'error');
            }
        });

        updateLocalItem(id, { top: isChecked });
    });

    $(document).on('change', '.forschung-toggle', function () {
        const id = $(this).data('id');
        const isChecked = $(this).is(':checked');

        $.ajax({
            url: BASE_PATH + 'Admin/MakerSpaceProject/ToggleForschung',
            type: 'POST',
            data: { id: id, isForschung: isChecked },
            success: function (response) {
                if (!response.success) {
                    Swal.fire('Fehler!', response.message, 'error');
                }
            },
            error: function () {
                Swal.fire('Fehler!', 'Ein Fehler ist aufgetreten.', 'error');
            }
        });

        updateLocalItem(id, { forschung: isChecked });
    });

    $(document).on('change', '.download-toggle', function () {
        const id = $(this).data('id');
        const isChecked = $(this).is(':checked');

        $.ajax({
            url: BASE_PATH + 'Admin/MakerSpaceProject/ToggleDownload',
            type: 'POST',
            data: { id: id, isDownload: isChecked },
            success: function (response) {
                if (!response.success) {
                    Swal.fire('Fehler!', response.message, 'error');
                }
            },
            error: function () {
                Swal.fire('Fehler!', 'Ein Fehler ist aufgetreten.', 'error');
            }
        });

        updateLocalItem(id, { download: isChecked });
    });

    $(document).on('change', '.tutorial-toggle', function () {
        const id = $(this).data('id');
        const isChecked = $(this).is(':checked');

        $.ajax({
            url:  BASE_PATH + 'Admin/MakerSpaceProject/ToggleTutorial',
            type: 'POST',
            data: { id: id, isTutorial: isChecked },
            success: function (response) {
                if (!response.success) {
                    Swal.fire('Fehler!', response.message, 'error');
                }
            },
            error: function () {
                Swal.fire('Fehler!', 'Ein Fehler ist aufgetreten.', 'error');
            }
        });

        updateLocalItem(id, { tutorial: isChecked });
    });

    $(document).on('change', '.netzwerk-toggle', function () {
        const id = $(this).data('id');
        const isChecked = $(this).is(':checked');

        $.ajax({
            url: BASE_PATH + 'Admin/MakerSpaceProject/ToggleNetzwerk',
            type: 'POST',
            data: { id: id, isNetzwerk: isChecked },
            success: function (response) {
                if (!response.success) {
                    Swal.fire('Fehler!', response.message, 'error');
                }
            },
            error: function () {
                Swal.fire('Fehler!', 'Ein Fehler ist aufgetreten.', 'error');
            }
        });

        updateLocalItem(id, { netzwerk: isChecked });
    });


    $(document).on('change', '.events-toggle', function () {
        const id = $(this).data('id');
        const isChecked = $(this).is(':checked');

        $.ajax({
            url:  BASE_PATH + 'Admin/MakerSpaceProject/ToggleEvent',
            type: 'POST',
            data: { id: id, isEvent: isChecked },
            success: function (response) {
                if (!response.success) {
                    Swal.fire('Fehler!', response.message, 'error');
                }
            },
            error: function () {
                Swal.fire('Fehler!', 'Ein Fehler ist aufgetreten.', 'error');
            }
        });

        updateLocalItem(id, { events: isChecked });
    });

    $(document).on('change', '.lesezeichen-toggle', function () {
        const id = $(this).data('id');
        const isChecked = $(this).is(':checked');

        $.ajax({
            url: BASE_PATH + 'Admin/MakerSpaceProject/ToggleLesezeichen',
            type: 'POST',
            data: { id: id, isLesezeichen: isChecked },
            success: function (response) {
                if (!response.success) {
                    Swal.fire('Fehler!', response.message, 'error');
                }
            },
            error: function () {
                Swal.fire('Fehler!', 'Ein Fehler ist aufgetreten.', 'error');
            }
        });

        updateLocalItem(id, { lesezeichen: isChecked });
    });

    $(document).on('change', '.ITRecht-toggle', function () {
        const id = $(this).data('id');
        const isChecked = $(this).is(':checked');

        $.ajax({
            url: BASE_PATH + 'Admin/MakerSpaceProject/ToggleITRecht',
            type: 'POST',
            data: { id: id, isITRecht: isChecked },
            success: function (response) {
                if (!response.success) {
                    Swal.fire('Fehler!', response.message, 'error');
                }
            },
            error: function () {
                Swal.fire('Fehler!', 'Ein Fehler ist aufgetreten.', 'error');
            }
        });

        updateLocalItem(id, { itRecht: isChecked });
    });


    $(document).on('change', '.beitraege-toggle', function () {
        const id = $(this).data('id');
        const isChecked = $(this).is(':checked');

        $.ajax({
            url: BASE_PATH + 'Admin/MakerSpaceProject/ToggleBeitraege',
            type: 'POST',
            data: { id: id, isBeitraege: isChecked },
            success: function (response) {
                if (!response.success) {
                    Swal.fire('Fehler!', response.message, 'error');
                }
            },
            error: function () {
                Swal.fire('Fehler!', 'Ein Fehler ist aufgetreten.', 'error');
            }
        });

        updateLocalItem(id, { beitraege: isChecked });
    });

    // DELETE‑Handler
    $(document).on('click', '.delete-project', function () {
        const id = $(this).data('id');

        Swal.fire({
            title: 'Projekt löschen?',
            text: 'Sind Sie sicher, dass Sie dieses Projekt unwiderruflich löschen möchten?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Ja, löschen!',
            cancelButtonText: 'Abbrechen'
        }).then(result => {
            if (!result.isConfirmed) return;

            /* ---------- HIER: neuen Aufruf einsetzen ---------- */
            $.ajax({
                url: BASE_PATH + 'admin/makerspaceproject/delete',   // → arbeitet lokal & online
                type: 'POST',
                data: {
                    id,
                    __RequestVerificationToken: CSRF_TOKEN             // Anti‑Forgery‑Token mitsenden
                },
                success: resp => {
                    if (resp.success) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Gelöscht!',
                            timer: 1800,
                            showConfirmButton: false
                        }).then(() => reloadAfterMutation());
                    } else {
                        Swal.fire('Fehler!', resp.message, 'error');
                    }
                },
                error: xhr => {
                    Swal.fire('Fehler!', xhr.responseText || 'Unbekannter Fehler', 'error');
                }
            });
            /* --------------------------------------------------- */
        });
    });


});