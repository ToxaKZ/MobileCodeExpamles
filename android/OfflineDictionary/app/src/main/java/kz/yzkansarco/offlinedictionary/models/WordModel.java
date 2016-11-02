package kz.yzkansarco.offlinedictionary.models;

public final class WordModel {
    private int id;
    private String name;
    private ArticleModel[] childrens;

    public WordModel() {
        id = 0;
        name = null;
        childrens = null;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public ArticleModel[] getChildrens() {
        return childrens;
    }

    public void setChildrens(ArticleModel[] childrens) {
        this.childrens = childrens;
    }
}