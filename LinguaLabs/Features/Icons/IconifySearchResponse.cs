namespace LinguaLabs.Features.Icons;

public partial record IconifySearchResponse
{
    public string[] Icons { get; set; } = [];
    
    public Dictionary<string, IconifyCollection> Collections { get; set; } = new();
    
    public IconifyRequest Request { get; set; } = new();
}

public partial record IconifyCollection
{
    public string Name { get; set; } = string.Empty;
    
    public int Total { get; set; }
    
    public string Version { get; set; } = string.Empty;
    
    public IconifyAuthor? Author { get; set; }
    
    public IconifyLicense? License { get; set; }
    
    public string[] Samples { get; set; } = [];
    
    public int Height { get; set; }
    
    public string Category { get; set; } = string.Empty;
    
    public string[] Tags { get; set; } = [];
    
    public bool Palette { get; set; }
}

public partial record IconifyAuthor
{
    public string Name { get; set; } = string.Empty;
    
    public string Url { get; set; } = string.Empty;
}

public partial record IconifyLicense
{
    public string Title { get; set; } = string.Empty;
    
    public string Spdx { get; set; } = string.Empty;
    
    public string Url { get; set; } = string.Empty;
}

public partial record IconifyRequest
{
    public string Query { get; set; } = string.Empty;
}
