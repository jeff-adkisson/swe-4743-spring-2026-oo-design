package inheritance.demo.entities;

public class PaperbackBook extends Publication {
    private final int pages;

    public PaperbackBook(String title, int pages) {
        super(title);
        this.pages = pages;
    }
}