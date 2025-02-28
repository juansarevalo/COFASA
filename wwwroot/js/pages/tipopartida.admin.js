'use strict';

var dmgDoctosGridSelect = '';
overrideIds({
    tableId: 'dmgDoctosDt',
    formId: 'dmgDoctosFormBody',
    formValuesId: 'dmgDoctosForm',
    addButtonId: 'addDmgDoctos',
    gridButtonsId: 'dmgDoctosGridButtons'
});
API.saveOneURL = '/TipoPartida/SaveOrUpdate';

$(document).ready(function () {
    prepareDmgDoctosSelect();
    overridePrepareButtons();
    overrideInitDt();
});

function overridePrepareButtons() {
    prepareButtons(function () {
        $(`#${initValues.addButtonId}`).click(function(){ overrideShowForm(); });
    });
}

function prepareDmgDoctosSelect() {
    const bodyButtons = $('#dmgDoctosGridSelect').val();
    const tags = $('<div/>');
    tags.append(bodyButtons);

    dmgDoctosGridSelect = '<center>' + tags.html() + '<center>';
}

function bindDmgDoctosSelect() {
    $(`#${initValues.dataTableId} tbody tr td select`).unbind('change').on('change', function (event) {
        if (event.preventDefault) { event.preventDefault(); }
        if (event.stopImmediatePropagation) { event.stopImmediatePropagation(); }

        const obj = JSON.parse(Base64.decode($(this).attr('data-row')));
        callDmgDoctosFromSelect(obj, $(this).val());
    });
}

function overrideInitDt() {
    initDt(
        function (table) {
            dtTable = table.DataTable({
                'ajax': {
                    'url': `/TipoPartida/GetDmgDoctos?ciaCod=${$('#codCia').val()}`,
                    'type': 'GET',
                    'datatype': 'JSON',
                },
                'columns': [
                    { 'data': 'IdTipoPartida'},
                    { 'data': 'CodCia'},
                    { 'data': 'TipoPartida' },
                    { 'data': 'TipoHomologar' },
                    { 'data': 'Nombre' },
                    {
                        sortable: false, searchable: false,
                        render: function (data, type, row) {
                            return gridButtons.replace('{data}',Base64.encode($.toJSON(row)));
                        }
                    }
                ]
            });
        },
        function () {
            bindDmgDoctosSelect();
            bindButtons(function (action, data) {
                if (action === 'edit') { console.log(data); overrideShowForm(data.IdTipoPartida); }
            });
        }
    );
}

/**
 * 
 * @param id TipoPartida.
 */
function overrideShowForm(id) {
    showForm(id, function (isEditing) {
        startDmgDoctosValidation();
        $('#CodCia').val($('#codCia').val());

        if (isEditing) {
            $('#IdTipoPartida').val(id);
            $('#btnSaveDmgDoctosText').text('Editar');
            overrideLoadOne(id);
        }
    });
}

function callDmgDoctosFromSelect(data, newStatus) {
    const formData = `isUpdating=1&COD_CIA=${data.COD_CIA}&TipoPartida=${data.TipoPartida}
        &Nombre=${data.Nombre}`;

    POST({
        url: '/TipoPartida/SaveOrUpdate',
        data: formData,
        success: function (data) {
            // Mostrar el mensaje (Éxito o error) devuelto por el servidor
            showToast(data.success, data.message);
            // Recargar tabla si fue exitoso
            if (data.success) reloadTable();
        },
        error: function (error) {
            console.log(error);
        }
    });
}


function startDmgDoctosValidation() {
    $(`#${initValues.formID}`).validate({
        rules: {
            isUpdating: {required: false, minlength: 1, maxlength: 2},
            CodCia: {required: true, minlength: 3, maxlength: 3},
            TipoPartida: { required: true, minlength: 2, maxlength: 3 },
            TipoHomologar: { required: true, minlength: 2, maxlength: 2 },
            Nombre: {required: true, minlength: 4, maxlength: 40},
        },
        submitHandler: function (form, event) {
            doSave({});
        }
    });
}

function overrideLoadOne(IdTipoPartida) {
    API.getOneURL = `/TipoPartida/GetOneDmgDocto?ciaCod=${$('#codCia').val()}&IdTipoPartida=${IdTipoPartida}`;

    loadOne({
        success: function (data) {
            if(data.success){ setDataToDmgDoctoForm(data.data); }
        }
    });
}

function setDataToDmgDoctoForm(data) {
    $('#IdTipoPartida').val(data.IdTipoPartida);
    $('#TipoPartida').val(data.TipoPartida);
    $('#TipoHomologar').val(data.TipoHomologar);
    $('#CONTADOR_POLIZA').val(data.CONTADOR_POLIZA);
    $('#Nombre').val(data.Nombre);
    $('#PROCESO').val(data.PROCESO);
    $('#POLIZA_MANUAL').val(data.POLIZA_MANUAL);
}