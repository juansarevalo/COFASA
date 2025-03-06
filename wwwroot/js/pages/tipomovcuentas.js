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
                    { 'data': 'TipoMov' },
                    { 'data': 'IdTipoMov' },
                    { 'data': 'IdCatalogo' },
                    { 'data': 'CentroCostoF' },
                    { 'data': 'TipoCuenta' },
                    { 'data': 'TipoPartida.TipoPartida' },
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
        'Nombre Movimiento...',
        false,
        function (term, page) {
            return {
                TipoMov: $('#TipoMov').val(),
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
        'Centros de Costo...'
    );

    initSelect2Paginated(
        'IdTipoPartida',
        '/TipoPartida/GetToSelect2',
        'Tipos de Asiento...'
    );

    if (isEditing) {
        $('#Id').val(id);
        $(`#${initValues.formAddButtonTextId}`).text('Editar');

        overrideLoadOne(id);
    }

    validateTipoMovFormSel2();
    $('#TipoMov').on('change', function () { validateTipoMovFormSel2(); });
    overrideFormValidation();
}

function validateTipoMovFormSel2() {
    if ($('#TipoMov').val() === '') {
        readOnlySelect2('#IdTipoMov', true);
        setSelect2Data('#IdTipoMov', null);
        $('#IdTipoMovLabel').text('Tipo...');
    } else {
        $('#IdTipoMovLabel').text('Tipo ' + $('#TipoMov').val());
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

    if (isDefined(data.TipoMov)) $('#TipoMov').val(data.TipoMov).trigger('change');


    //Tipo entrada/salida cofasa
    const tipoMovFetch = await fetch(`/TipoMovCuentas/GetCofasaTipoMovData?TipoMov=${data.TipoMov}&IdTipoMov=${data.IdTipoMov}`);
    const tipoMov = await tipoMovFetch.json()

    if (isDefined(data.IdTipoMov)) $('#IdTipoMov').select2('data', {
        id: tipoMov.data.idTipoMov,
        text: tipoMov.data.idTipoMov + " - " + tipoMov.data.nombre
    }).change();


    //Catalogo cofasa
    const catalogoFetch = await fetch("/DmgCuentas/GetCofasaCatalogoDataById?id=" + data.IdCatalogo);
    const catalogo = await catalogoFetch.json()

    if (isDefined(data.IdCatalogo)) $('#IdCatalogo').select2('data', {
        id: catalogo.data.idCatalogo,
        text: catalogo.data.codContable + " - " + catalogo.data.DESCRIP_ESP
    }).change();


    if (isDefined(data.CentroCosto)) $('#CentroCostoF').select2('data', {
        id: data.CentroCostoF,
        text: data.CentroCostoF + " - " + data.CentroCosto.DESCRIPCION
    }).change();
    if (isDefined(data.TipoPartida)) $('#IdTipoPartida').select2('data', {
        id: data.IdTipoPartida,
        text: data.TipoPartida.TipoPartida + " - " + data.TipoPartida.Nombre
    }).change();
    if (isDefined(data.TipoCuenta)) $('#TipoCuenta').val(data.TipoCuenta);
    if (isDefined(data.FormaCalculo)) $('#FormaCalculo').val(data.FormaCalculo);
}