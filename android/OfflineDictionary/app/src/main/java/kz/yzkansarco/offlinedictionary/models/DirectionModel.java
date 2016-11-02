package kz.yzkansarco.offlinedictionary.models;

public final class DirectionModel {
    private int id;
    private String fromCulture;
    private String toCulture;
    private String fromLocale;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getFromCulture() {
        return fromCulture;
    }

    public void setFromCulture(String fromCulture) {
        this.fromCulture = fromCulture;
    }

    public String getToCulture() {
        return toCulture;
    }

    public void setToCulture(String toCulture) {
        this.toCulture = toCulture;
    }

    public String getFromLocale() {
        return fromLocale;
    }

    public void setFromLocale(String fromLocale) {
        this.fromLocale = fromLocale;
    }
    
    @Override
    public String toString() {
        return getDisplayDirection(fromCulture, toCulture);
    }

    public static String getDisplayDirection(final String fromCulture, final String toCulture) {
        return String.format("%s-%s", fromCulture, toCulture);
    }
}