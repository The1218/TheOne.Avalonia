namespace TheOne.Avalonia.Controls;

/// <summary>The built-in TheOne glyphs. Numeric identities are stable; new glyphs are appended.</summary>
public enum OneIconKind
{
    Add = 0, Remove = 1, Edit = 2, Delete = 3, Search = 4, Save = 5, Refresh = 6, More = 7,
    Home = 8, Menu = 9, Back = 10, Forward = 11, Up = 12, Down = 13, Expand = 14, Collapse = 15,
    File = 16, Folder = 17, Document = 18, Image = 19, Upload = 20, Download = 21, Copy = 22, Link = 23,
    User = 24, Users = 25, Role = 26, Lock = 27, Unlock = 28, Shield = 29, Login = 30, Logout = 31,
    Info = 32, Success = 33, Warning = 34, Error = 35, Question = 36, Bell = 37, Message = 38, Loading = 39,
    Table = 40, Database = 41, LineChart = 42, BarChart = 43, PieChart = 44, Dashboard = 45, Filter = 46, Sort = 47,
    Calendar = 48, Clock = 49, DateRange = 50, Timer = 51, History = 52, Schedule = 53, PreviousDay = 54, NextDay = 55,
    Window = 56, Tabs = 57, Sidebar = 58, Grid = 59, List = 60, Split = 61, Maximize = 62, Minimize = 63,
    Settings = 64, Monitor = 65, Mobile = 66, Printer = 67, Keyboard = 68, Mouse = 69, Sun = 70, Moon = 71,
    Code = 72, Terminal = 73, Api = 74, Server = 75, Cloud = 76, Network = 77, Bug = 78, Package = 79,
    Start = 80, End = 81, Approval = 82, Task = 83, Branch = 84, Parallel = 85, Merge = 86, Workflow = 87,
    Pointer = 88, Text = 89, Rectangle = 90, Line = 91, AlignLeft = 92, AlignCenter = 93, Barcode = 94, QrCode = 95,
    Close = 96, Undo = 97, Redo = 98, Share = 99,
    ChevronLeft = 100, ChevronRight = 101, ChevronUp = 102, ChevronDown = 103,
    FileAdd = 104, FolderOpen = 105, Attachment = 106, FileSearch = 107,
    UserAdd = 108, UserRemove = 109, Key = 110, Permission = 111,
    BellOff = 112, Mail = 113, Send = 114, Flag = 115,
    SortAscending = 116, SortDescending = 117, Statistics = 118, TrendUp = 119,
    Alarm = 120, CalendarCheck = 121, CalendarAdd = 122, CalendarRemove = 123,
    Fullscreen = 124, FullscreenExit = 125, Pin = 126, Unpin = 127,
    Play = 128, Pause = 129, Stop = 130, Volume = 131,
    Wifi = 132, WifiOff = 133, Connection = 134, Disconnect = 135,
    Approve = 136, Reject = 137, Transfer = 138, Pending = 139,
    ZoomIn = 140, ZoomOut = 141, AlignRight = 142, Crop = 143,
    Cut = 144, Paste = 145, PrintPreview = 146, Export = 147,
    Compass = 148, Location = 149, Route = 150, BreadcrumbPath = 151,
    FileCheck = 152, FileLock = 153, Archive = 154, TrashRestore = 155,
    UserCheck = 156, UserSettings = 157, Organization = 158, Fingerprint = 159,
    NotificationRead = 160, ChatReply = 161, Heart = 162, Star = 163,
    ScatterPlot = 164, AreaChart = 165, Heatmap = 166, DataImport = 167,
    ClockCheck = 168, ClockForward = 169, CalendarWeek = 170, TimeZone = 171,
    DockBottom = 172, PanelLeftClose = 173, PanelLeftOpen = 174, Layers = 175,
    Camera = 176, Microphone = 177, Headphones = 178, Battery = 179,
    GitBranch = 180, GitCommit = 181, Braces = 182, Container = 183,
    Reassign = 184, ApprovalChain = 185, WorkflowValidate = 186, WorkflowPublish = 187,
    AlignTop = 188, AlignBottom = 189, DistributeHorizontal = 190, Palette = 191,
    Wallet = 192, CreditCard = 193, Bank = 194, Cash = 195,
    Coins = 196, Receipt = 197, Invoice = 198, Calculator = 199,
    Payment = 200, Refund = 201, Exchange = 202, CurrencyYuan = 203,
    CurrencyDollar = 204, Safe = 205, FinancialReport = 206, StockTrend = 207,
    Factory = 208, Gear = 209, Wrench = 210, Hammer = 211,
    RobotArm = 212, Conveyor = 213, Valve = 214, Pump = 215,
    Motor = 216, Sensor = 217, Gauge = 218, Crane = 219,
    Forklift = 220, HardHat = 221, Welding = 222, Toolbox = 223,
    MedicalCross = 224, Hospital = 225, Stethoscope = 226, HeartPulse = 227,
    Pill = 228, Syringe = 229, Thermometer = 230, FirstAid = 231,
    Ambulance = 232, Wheelchair = 233, Microscope = 234, TestTube = 235,
    Dna = 236, Lungs = 237, Tooth = 238, Bandage = 239,
    Building = 240, Bridge = 241, Road = 242, Railway = 243,
    Airport = 244, PowerTower = 245, PowerPlug = 246, WaterDrop = 247,
    WaterPipe = 248, WindTurbine = 249, SolarPanel = 250, StreetLight = 251,
    TrafficLight = 252, FireHydrant = 253, Warehouse = 254, Recycle = 255,
    Truck = 256, DeliveryVan = 257, Car = 258, Bus = 259,
    Bicycle = 260, Train = 261, Ship = 262, FlightTakeoff = 263,
    DeliveryBox = 264, DeliveryTracking = 265, Map = 266, NavigationArrow = 267,
    Parking = 268, Fuel = 269, Anchor = 270, TrafficCone = 271,
    ShoppingCart = 272, ShoppingBag = 273, Store = 274, TagLabel = 275,
    Discount = 276, Gift = 277, Product = 278, Scan = 279,
    ShoppingBasket = 280, Order = 281, OrderCheck = 282, OrderCancel = 283,
    Coupon = 284, LoyaltyCard = 285, CustomerService = 286, ShoppingCompare = 287,
    Book = 288, BookOpen = 289, GraduationCap = 290, School = 291,
    Pen = 292, Notebook = 293, Library = 294, Certificate = 295,
    Lightbulb = 296, Flask = 297, Ruler = 298, Globe = 299,
    Language = 300, Presentation = 301, ReadAloud = 302, Backpack = 303,
    CloudSun = 304, CloudMoon = 305, Rain = 306, Snow = 307,
    Thunderstorm = 308, Wind = 309, Fog = 310, Tornado = 311,
    Rainbow = 312, Umbrella = 313, TemperatureHigh = 314, Leaf = 315,
    Tree = 316, Flower = 317, Mountain = 318, Earth = 319
}

/// <summary>Semantic groups in the built-in glyph catalog.</summary>
public enum OneIconCategory
{
    Actions, Navigation, Files, People, Feedback, Data, DateTime, Layout, Devices, Development, Workflow, Design,
    Finance, Industry, Medical, Infrastructure, Transport, Commerce, Education, Weather
}

/// <summary>Filled uses a dedicated silhouette when provided; otherwise the outline is retained.</summary>
public enum OneIconVariant { Outline, Filled }
