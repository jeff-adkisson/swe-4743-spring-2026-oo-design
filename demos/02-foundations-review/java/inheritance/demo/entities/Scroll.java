package inheritance.demo.entities;

public class Scroll extends Publication {
    private final int lengthMillimeters;

    public Scroll(String title, int lengthMillimeters) {
        super(title);
        this.lengthMillimeters = lengthMillimeters;
    }
}