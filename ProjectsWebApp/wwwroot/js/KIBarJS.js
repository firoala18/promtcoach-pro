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
$(document).ready(function () {

    // ========== CARD VIEW FUNCTIONALITY ==========
    let allData = [];
    let filteredData = [];
    let currentPage = 1;
    let pageSize = 12;

    const cardsContainer = document.getElementById('kibarCards');
    const emptyContainer = document.getElementById('kibarCardsEmpty');
    const paginationWrap = document.getElementById('kibarPaginationWrap');
    const paginationEl = document.getElementById('kibarPagination');
    const searchInput = document.getElementById('kibarSearch');
    const pageSizeSelect = document.getElementById('kibarPageSize');
    const countEl = document.getElementById('kibarCount');

    function escapeHtml(str) {
        if (!str) return '';
        return String(str).replace(/[&<>"']/g, m => ({
            '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;'
        }[m]));
    }

    function renderCards() {
        if (!cardsContainer) return;

        const start = (currentPage - 1) * pageSize;
        const end = start + pageSize;
        const pageData = filteredData.slice(start, end);

        if (pageData.length === 0) {
            cardsContainer.innerHTML = '';
            if (emptyContainer) emptyContainer.classList.remove('d-none');
            if (paginationWrap) paginationWrap.classList.add('d-none');
            if (countEl) countEl.textContent = '0 Einträge';
            return;
        }

        if (emptyContainer) emptyContainer.classList.add('d-none');
        if (countEl) countEl.textContent = `${filteredData.length} Einträge`;

        const upsertBase = window.UPSERT_BASE || (BASE_PATH + 'admin/makerspaceproject/upsert');

        cardsContainer.innerHTML = pageData.map(item => {
            const imgSrc = toAppUrl(item.imageUrl) || '';
            const editUrl = `${upsertBase}?id=${encodeURIComponent(item.id)}`;
            const tags = item.tags ? item.tags.split(',').map(t => t.trim()).filter(Boolean) : [];

            return `
            <div class="kibar-item" data-id="${item.id}">
                <div class="kibar-item-media">
                    ${imgSrc ? `<img src="${escapeHtml(imgSrc)}" alt="${escapeHtml(item.title)}" onerror="this.style.display='none'">` : '<i class="bi bi-image text-muted" style="font-size:2.5rem"></i>'}
                </div>
                <div class="kibar-item-body">
                    <div class="kibar-item-meta">
                        <span class="kibar-order"><i class="bi bi-arrow-down-up"></i> ${item.displayOrder || 0}</span>
                        <span class="text-muted small">#${item.id}</span>
                    </div>
                    <h5 class="kibar-item-title">${escapeHtml(item.title)}</h5>
                    ${tags.length ? `<div class="kibar-tags">${tags.map(t => `<span class="badge">${escapeHtml(t)}</span>`).join('')}</div>` : ''}
                    <p class="kibar-item-desc">${escapeHtml(item.description)}</p>
                    <div class="kibar-item-toggles">
                        <div class="form-check form-switch"><input class="form-check-input lesezeichen-toggle" type="checkbox" data-id="${item.id}" ${item.lesezeichen ? 'checked' : ''}><label class="form-check-label small">Tipp</label></div>
                        <div class="form-check form-switch"><input class="form-check-input forschung-toggle" type="checkbox" data-id="${item.id}" ${item.forschung ? 'checked' : ''}><label class="form-check-label small">Forschung</label></div>
                        <div class="form-check form-switch"><input class="form-check-input top-toggle" type="checkbox" data-id="${item.id}" ${item.top ? 'checked' : ''}><label class="form-check-label small">Lehre</label></div>
                        <div class="form-check form-switch"><input class="form-check-input events-toggle" type="checkbox" data-id="${item.id}" ${item.events ? 'checked' : ''}><label class="form-check-label small">Lernen</label></div>
                        <div class="form-check form-switch"><input class="form-check-input tutorial-toggle" type="checkbox" data-id="${item.id}" ${item.tutorial ? 'checked' : ''}><label class="form-check-label small">Tutorial</label></div>
                        <div class="form-check form-switch"><input class="form-check-input ITRecht-toggle" type="checkbox" data-id="${item.id}" ${item.itRecht ? 'checked' : ''}><label class="form-check-label small">IT-Recht</label></div>
                        <div class="form-check form-switch"><input class="form-check-input beitraege-toggle" type="checkbox" data-id="${item.id}" ${item.beitraege ? 'checked' : ''}><label class="form-check-label small">Beiträge</label></div>
                    </div>
                </div>
                <div class="kibar-item-actions">
                    <a href="${escapeHtml(item.projectUrl)}" target="_blank" class="btn kibar-open-btn" title="Öffnen"><i class="bi bi-box-arrow-up-right"></i></a>
                    <div class="d-flex kibar-action-buttons">
                        <a href="${editUrl}" class="btn btn-warning" title="Bearbeiten"><i class="bi bi-pencil-square"></i></a>
                        <button class="btn btn-danger delete-project" data-id="${item.id}" title="Löschen"><i class="bi bi-trash-fill"></i></button>
                    </div>
                </div>
            </div>`;
        }).join('');

        renderPagination();
    }

    function renderPagination() {
        if (!paginationEl || !paginationWrap) return;

        const totalPages = Math.ceil(filteredData.length / pageSize);
        if (totalPages <= 1) {
            paginationWrap.classList.add('d-none');
            return;
        }

        paginationWrap.classList.remove('d-none');
        let html = '';

        html += `<li class="page-item ${currentPage === 1 ? 'disabled' : ''}"><a class="page-link" href="#" data-page="${currentPage - 1}">&laquo;</a></li>`;

        for (let i = 1; i <= totalPages; i++) {
            if (i === 1 || i === totalPages || (i >= currentPage - 2 && i <= currentPage + 2)) {
                html += `<li class="page-item ${i === currentPage ? 'active' : ''}"><a class="page-link" href="#" data-page="${i}">${i}</a></li>`;
            } else if (i === currentPage - 3 || i === currentPage + 3) {
                html += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
            }
        }

        html += `<li class="page-item ${currentPage === totalPages ? 'disabled' : ''}"><a class="page-link" href="#" data-page="${currentPage + 1}">&raquo;</a></li>`;

        paginationEl.innerHTML = html;
    }

    function filterData(query) {
        const q = (query || '').toLowerCase().trim();
        if (!q) {
            filteredData = [...allData];
        } else {
            filteredData = allData.filter(item =>
                (item.title && item.title.toLowerCase().includes(q)) ||
                (item.description && item.description.toLowerCase().includes(q)) ||
                (item.tags && item.tags.toLowerCase().includes(q))
            );
        }
        currentPage = 1;
        renderCards();
    }

    function loadData() {
        $.ajax({
            url: BASE_PATH + 'admin/makerspaceproject/getall',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                allData = response.data || [];
                allData.sort((a, b) => (a.displayOrder || 0) - (b.displayOrder || 0));
                filteredData = [...allData];
                renderCards();
            },
            error: function () {
                if (emptyContainer) {
                    emptyContainer.textContent = 'Fehler beim Laden der Daten';
                    emptyContainer.classList.remove('d-none');
                }
            }
        });
    }

    // Event listeners
    if (searchInput) {
        searchInput.addEventListener('input', function () {
            filterData(this.value);
        });
    }

    if (pageSizeSelect) {
        pageSizeSelect.addEventListener('change', function () {
            pageSize = parseInt(this.value, 10) || 12;
            currentPage = 1;
            renderCards();
        });
    }

    if (paginationEl) {
        paginationEl.addEventListener('click', function (e) {
            e.preventDefault();
            const link = e.target.closest('a[data-page]');
            if (!link) return;
            const page = parseInt(link.dataset.page, 10);
            const totalPages = Math.ceil(filteredData.length / pageSize);
            if (page >= 1 && page <= totalPages) {
                currentPage = page;
                renderCards();
                cardsContainer.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        });
    }

    // Load data on page load
    loadData();
    // ========== END CARD VIEW FUNCTIONALITY ==========

    dataTable1 = $('#tblMakerSpace').DataTable({ // ✅ assign it here
      
        "ajax": {
            url: BASE_PATH + 'admin/makerspaceproject/getall',
            type: 'GET',
            datatype: 'json'
        },
        "columns": [
            { data: 'verlauf', width: "3%" },
            {
                data: 'imageUrl',
                render: function (data) {
                    const src = toAppUrl(data);
                    return `
    <img src="${src}" style="width: 100px; height: 60px; object-fit: contain; background-color: #f9f9f9; border-radius: 6px;" />`;
                },
                width: "10%"
            },

            { data: 'title', width: "20%" },

            {
                data: 'lesezeichen',
                render: function (data, type, row) {
                    const checked = data ? 'checked' : '';
                    return `<div class="form-check form-switch text-center">
        <input class="form-check-input lesezeichen-toggle" type="checkbox" data-id="${row.id}" ${checked}>
    </div>`;
                },
                width: "8%"
            },

            {
                data: 'forschung',
                render: function (data, type, row) {
                    const checked = data ? 'checked' : '';
                    return `<div class="form-check form-switch text-center">
        <input class="form-check-input forschung-toggle" type="checkbox" data-id="${row.id}" ${checked}>
    </div>`;
                },
                width: "8%"
            },

            {
                data: 'top',
                render: function (data, type, row) {
                    const checked = data ? 'checked' : '';
                    return `<div class="form-check form-switch text-center">
        <input class="form-check-input top-toggle" type="checkbox" data-id="${row.id}" ${checked}>
    </div>`;
                },
                width: "8%"
            },

            {
                data: 'events',
                render: function (data, type, row) {
                    const checked = data ? 'checked' : '';
                    return `<div class="form-check form-switch text-center">
        <input class="form-check-input events-toggle" type="checkbox" data-id="${row.id}" ${checked}>
    </div>`;
                },
                width: "8%"
            },

            {
                data: 'tutorial',
                render: function (data, type, row) {
                    const checked = data ? 'checked' : '';
                    return `<div class="form-check form-switch text-center">
        <input class="form-check-input tutorial-toggle" type="checkbox" data-id="${row.id}" ${checked}>
    </div>`;
                },
                width: "8%"
            },
            {
                data: 'itRecht',
                render: function (data, type, row) {
                    const checked = data ? 'checked' : '';
                    return `<div class="form-check form-switch text-center">
        <input class="form-check-input ITRecht-toggle" type="checkbox" data-id="${row.id}" ${checked}>
    </div>`;
                },
                width: "8%"
            },
            {
                data: 'beitraege',
                render: function (data, type, row) {
                    const checked = data ? 'checked' : '';
                    return `<div class="form-check form-switch text-center">
        <input class="form-check-input beitraege-toggle" type="checkbox" data-id="${row.id}" ${checked}>
    </div>`;
                },
                width: "8%"
            },



            {
                data: 'id',
                render: function (data) { // ✅ Single render function
                    const base = (window.UPSERT_BASE && typeof window.UPSERT_BASE === 'string')
                        ? window.UPSERT_BASE
                        : (BASE_PATH + 'admin/makerspaceproject/upsert');
                    const href = `${base}?id=${encodeURIComponent(data)}`;
                    return `
    <div class="d-flex justify-content-center">
        <a href="${href}" class="btn btn-warning mx-2">
            <i class="bi bi-pencil-square"></i>
        </a>
        <button class="btn btn-danger mx-2 delete-project" data-id="${data}">
            <i class="bi bi-trash-fill"></i>
        </button>
    </div>`;
                },
                width: "10%"
            }
        ],

        "language": {
            "emptyTable": "Keine Daten verfügbar",
            "search": "Suchen:",
            "lengthMenu": "Zeige _MENU_ Einträge",
            "info": "Zeige _START_ bis _END_ von _TOTAL_ Einträgen",
            "paginate": {
                "next": "Nächste",
                "previous": "Vorherige"
            }
        },
        
        "order": [[0, "asc"]],
        "pageLength": 50,
        "stateSave": true,
        initComplete: function () {
            this.api().page.len(50).draw(false);
        }
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
                        }).then(() => dataTable1.ajax.reload(null, false));   // Tabelle neu laden
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