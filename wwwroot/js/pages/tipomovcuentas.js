'use strict';

overrideIds({
    tableId: 'tipoMovCuentasDt',
    formId: 'tipoMovCuentasFormBody',
    addButtonId: 'tipoMovCuentasAddBtn',
    gridButtonsId: 'tipoMovCuentasGridButtons',
    formValuesId: 'tipoMovCuentasForm',
    btnSaveId: 'tipoMovCuentasBtnSave',
    btnSaveTextId: 'tipoMovCuentasFormBtnSaveText',
})

API.tableURL = '/TipoMovCuentas/GetAll?CodCia={CodCia}';
API.saveOneURL = '/TipoMovCuentas/SaveOrUpdate';
API.getOneURL = '/TipoMovCuentas/GetOne?Id={Id}';

$(document).ready(function () {
    overridePrepareButtons();
    overrideInitDt();
});

function overridePrepareButtons() {
    prepareButtons(function () {
        $(`#${initValues.addButtonId}`).click(function(){ overrideShowForm(); });
    });
}

function overrideInitDt() {
    initDt(
        function (table) {
            dtTable = table.DataTable({
                'ajax': {
                    'url': API.tableURL.replace('{CodCia}', $('#codCia').val()),
                    'type': 'GET',
                    'datatype': 'JSON',
                },
                'columns': [
                    { 'data': 'Id' },
                    {
                        'data': 'NombreMov',
                        render: function (data, type, row) {
                            if (data == 'Entrada') {
                                return data
                            } else if (data == 'SalidaC') {
                                return 'Salida Costo'
                            } else if (data == 'SalidaI') {
                                return 'Salida Ingreso'
                            }
                        }
                    },
                    { 'data': 'NombreTipoMov' },
                    { 'data': 'codContable' },
                    { 'data': 'CentroCostoF' },
                    { 'data': 'TipoCuenta' },
                    { 'data': 'TipoPartida' },
                    { 'data': 'FormaCalculo' },
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
            bindButtons(function (action, data) {
                if (action === 'edit') { overrideShowForm(data.Id); }
            });
        }
    );
}

function overrideShowForm(id) {
    const isEditing = !(typeof (id) === 'undefined' || id === 0);
    var title = '';

    if (isEditing) {
        title = 'Editar registro';
    } else {
        title = 'Agregar registro';
    }

    dialog = bootbox.dialog({
        title: title,
        message: $('#tipoMovCuentasFormBody').val(),
        className: 'modalLarge',
        onEscape: true
    });

    $('#CodCia').val($('#codCia').val());

    //Seleccionar tipo entrada/salida
    initSelect2Paginated(
        'IdTipoMov',
        '/TipoMovCuentas/GetToSelect2CofasaIdTipoMov',
        'Tipo Movimiento...',
        false,
        function (term, page) {
            return {
                NombreMov: $('#NombreMov').val(),
                q: term,
                page: page || 1,
                pageSize: 10
            };
        }
    );

    initSelect2Paginated(
        'IdCatalogo',
        '/DmgCuentas/GetToSelect2CofasaCatalogo',
        'Cuentas...'
    );

    initSelect2Paginated(
        'CentroCostoF',
        '/CentroCosto/GetToSelect2',
        'Centros de Costos...'
    );

    initSelect2Paginated(
        'IdTipoPartida',
        '/TipoPartida/GetToSelect2',
        'Nombres de Asiento...'
    );

    initSelect2Paginated(
        'IdPais',
        '/TipoMovCuentas/GetToSelect2CofasaIdPais',
        'Pais...'
    );

    if (isEditing) {
        $('#Id').val(id);
        $(`#${initValues.formAddButtonTextId}`).text('Editar');

        overrideLoadOne(id);
    }

    validateTipoMovFormSel2();
    $('#NombreMov').on('change', function () { validateTipoMovFormSel2(); });
    overrideFormValidation();
}

function validateTipoMovFormSel2() {
    if ($('#NombreMov').val() === '') {
        readOnlySelect2('#IdTipoMov', true);
        setSelect2Data('#IdTipoMov', null);
        $('#IdTipoMovLabel').text('Tipo...');
    } else {
        let NombreMov = $('#NombreMov').val();
        let NombreSelect = 'Tipo ';

        if (NombreMov == 'Entrada') {
            NombreSelect += NombreMov;
        } else if (NombreMov == 'SalidaC') {
            NombreSelect += 'Salida al Costo';
        } else if (NombreMov == 'SalidaI') {
            NombreSelect += 'Reconocimiento de Ingresos';
        } else if (NombreMov == 'SalidaCE') {
            NombreSelect += 'Salida al Costo Especial';
        }

        $('#IdTipoMovLabel').text(NombreSelect);
        readOnlySelect2('#IdTipoMov', false);
        setSelect2Data('#IdTipoMov', null);
    }
}

function overrideFormValidation() {
    initFormValidation({
        showErrorsCb: function (errorMap, errorList, validator) { validator.defaultShowErrors(); },
        submitHandlerCb: function (form, event) { doSave({}); }
    });
}

function overrideLoadOne(id) {
    const newURL = API.getOneURL
        .replaceAll('{Id}', id);

    loadOne({
        url: newURL,
        success: function (data) {
            if(data.success){ setDataToForm(data.data); }
        }
    });
}

async function setDataToForm(data) {
    if (!isDefined(data)) return;

    if (isDefined(data.NombreMov)) $('#NombreMov').val(data.NombreMov).trigger('change');

    //Tipo entrada/salida cofasa
    if (isDefined(data.IdTipoMov)) $('#IdTipoMov').select2('data', {
        id: data.IdTipoMov,
        text: data.IdTipoMov + " - " + data.NombreTipoMov
    }).change();


    //Catalogo cofasa
    if (isDefined(data.IdCatalogo)) $('#IdCatalogo').select2('data', {
        id: data.IdCatalogo,
        text: data.codContable + " - " + data.NombreCatalogo
    }).change();


    if (isDefined(data.CentroCostoF)) $('#CentroCostoF').select2('data', {
        id: data.CentroCostoF,
        text: data.CentroCostoF + " - " + data.NombreCentroCosto
    }).change();

    if (isDefined(data.TipoPartida)) $('#IdTipoPartida').select2('data', {
        id: data.IdTipoPartida,
        text: data.IdTipoPartida + " - " + data.TipoPartida
    }).change();
    if (isDefined(data.TipoCuenta)) $('#TipoCuenta').val(data.TipoCuenta);
    if (isDefined(data.FormaCalculo)) $('#FormaCalculo').val(data.FormaCalculo);

    if (isDefined(data.TipoPartida)) $('#IdPais').select2('data', {
        id: data.IdPais,
        text: data.IdPais + " - " + data.NombrePais
    }).change();
    if (isDefined(data.RetencionIVA)) data.RetencionIVA == "S" && $('#RetencionIVA').prop('checked', true)
}