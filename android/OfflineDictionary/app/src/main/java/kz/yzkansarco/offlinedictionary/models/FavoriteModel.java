package kz.yzkansarco.offlinedictionary.models;

public final class FavoriteModel {
    private int wordId;
    private String name;
    private int directionId;
    private String fromCulture;
    private String toCulture;

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public int getWordId() {
        return wordId;
    }

    public void setWordId(int wordId) {
        this.wordId = wordId;
    }

    public int getDirectionId() {
        return directionId;
    }

    public void setDirectionId(int directionId) {
        this.directionId = directionId;
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
}