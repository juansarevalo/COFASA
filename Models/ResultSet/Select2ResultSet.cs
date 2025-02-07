namespace CoreContable.Models.ResultSet {
    public class Select2ResultSet {
        public string id { get; set; }
        public string text { get; set; }
        public bool more { get; set; } // Aseg�rate de que esta propiedad est� presente

        // Propiedad calculada para formatear el texto con ceros
        public string FormattedText {
            get {
                return AddLeadingZeros(this.text); // Aplicar ceros
            }
        }

        // Funci�n para agregar ceros a la izquierda
        private string AddLeadingZeros(string accountCode) {
            const int totalLength = 10;  // Longitud deseada
            return accountCode.PadLeft(totalLength, '0'); // A�ade ceros a la izquierda
        }
    }
}
