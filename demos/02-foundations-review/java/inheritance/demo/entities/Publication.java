package inheritance.demo.entities;

public abstract class Publication {
    protected final String title;

    protected Publication(String title) {
        this.title = title;
    }

    public String getDetails() {
        return title;
    }
}